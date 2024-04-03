using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using OrdersAPI.Data;
using OrdersAPI.Models;
using OrdersAPI.Models.Dto;
using OrdersAPI.Models.DTO;
using OrdersAPI.Service.IService;
using OrdersAPI.Utility;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Azure;

namespace OrdersAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        private IBookService _bookService;
        private ICartService _cartService;
        private readonly IHttpClientFactory _httpClientFactory;


        public OrderAPIController(ApplicationDbContext db, IMapper mapper, IBookService bookService, ICartService cartService, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
            _bookService = bookService;
            _cartService = cartService;
            _httpClientFactory = httpClientFactory;
        }

        [Authorize]
        [HttpGet("Orders/{userId}")]
        public  async Task<ResponseDto?> Orders(string userId, int pageSize = 4, int pageNumber = 1)
        {
            //determine the role of the user
            var client = _httpClientFactory.CreateClient("Auth");
            var res =  await client.GetAsync($"/api/auth/role?userId={userId}");
            var apiContent = await res.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess == false) //role is customer
            {
                var totalUserOrders = _db.orderHeads.Count(u => u.UserId == userId);

                var userOrders = _db.orderHeads.Include(u => u.OrderDetail)
                                                .Where(u => u.UserId == userId)
                                                .OrderByDescending(u => u.OrderHeadId)
                                                .Skip((pageNumber - 1) * pageSize)
                                                .Take(pageSize)
                                                .ToList();

                var totalPages = (int)Math.Ceiling(totalUserOrders / (double)pageSize);
                _response.TotalPages = totalPages;
                _response.Result = _mapper.Map<IEnumerable<OrderHeadDto>>(userOrders);

                return _response;
            }
            else //role is admin
            {
                IEnumerable<OrderHead> objList = _db.orderHeads.OrderByDescending(u => u.OrderHeadId).ToList();

                int totalItems = objList.Count();
                int totalPages = (int)System.Math.Ceiling(totalItems / (double)pageSize);
                objList = objList.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
                var response = new ResponseDto
                {
                    Result = _mapper.Map<IEnumerable<OrderHeadDto>>(objList),
                    TotalPages = totalPages
                };
                return response;
            }
        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public ResponseDto? Get(int id)
        {
            var orderHeader = _db.orderHeads.Include(u => u.OrderDetail).First(u => u.OrderHeadId == id);
            _response.Result = _mapper.Map<OrderHeadDto>(orderHeader);

            return _response;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartsDto cartdto)
        {
            var userId = cartdto.CartHead.UserId;
            var userEmail = cartdto.CartHead.Email;

            //error handling
            var client = _httpClientFactory.CreateClient("Auth");
            var content = new StringContent(JsonConvert.SerializeObject(userId), Encoding.UTF8, "application/json");
            var response = await client.GetAsync($"/api/auth/checkuser?userId={userId}");

            if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception($"Could not find {userId}");
            }
            //error handling
            var res = await client.GetAsync($"/api/auth/checkmail?userId={userId}&email={userEmail}");

            if (res.StatusCode == HttpStatusCode.NoContent || res.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception($"Could not find {userEmail}");
            }

            OrderHeadDto orderHeaderDto = _mapper.Map<OrderHeadDto>(cartdto.CartHead);
            orderHeaderDto.Status = SD.Status_Pending;
            orderHeaderDto.OrderDetail = _mapper.Map<IEnumerable<OrderDetailDto>>(cartdto.CartDetail);

            //create order
            OrderHead orderCreated = _db.orderHeads.Add(_mapper.Map<OrderHead>(orderHeaderDto)).Entity;
            await _db.SaveChangesAsync();
            orderHeaderDto.OrderHeadId = orderCreated.OrderHeadId;
            _response.Result = orderHeaderDto;
            await _cartService.ClearCart(userId);

            return _response;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            var orderHeader = _db.orderHeads.Include(o => o.OrderDetail).FirstOrDefault(u => u.OrderHeadId == orderId);

            if (orderHeader == null)
            {
                throw new Exception("Invalid Order ID");
            }

            if (!(newStatus == "Approved" || newStatus == "Canceled"))
            {
                throw new Exception("Approved or canceled not entered");
            }

            if (newStatus == SD.Status_Cancelled)
            {
                foreach (var orderDetail in orderHeader.OrderDetail)
                {
                    var book = await _bookService.GetBook(orderDetail.ProductId);
                    if (book != null)
                    {
                        book.ProductStockQuantity += orderDetail.Count;
                        await _bookService.UpdateBook(book);
                    }
                }
            }
            orderHeader.Status = newStatus;
            _db.SaveChanges();

            return _response;
        }
    }
}