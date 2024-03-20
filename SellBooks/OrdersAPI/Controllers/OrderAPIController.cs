using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrdersAPI.Data;
using OrdersAPI.Models;
using OrdersAPI.Models.Dto;
using OrdersAPI.Service.IService;
using OrdersAPI.Utility;

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
        private readonly IValidator<CartsDto> _validator;
        public OrderAPIController(ApplicationDbContext db, IMapper mapper, IBookService bookService, ICartService cartService,IValidator<CartsDto> validator)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
            _bookService = bookService;
            _cartService = cartService;
            _validator = validator;
        }


        [Authorize]
        [HttpGet("GetOrders")]
        public ResponseDto? Get(string? userId, int pageSize = 4, int pageNumber = 1)
        {
            try
            {
                var allOrders = _db.orderHeads.Include(u => u.OrderDetail)
                                            .OrderByDescending(u => u.OrderHeadId)
                                             .Skip((pageNumber - 1) * pageSize)
                                             .Take(pageSize)
                                            .ToList();

                var totalItems = _db.orderHeads.Count();
                var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
                _response.TotalPages = totalPages;
                _response.Result = _mapper.Map<IEnumerable<OrderHeadDto>>(allOrders);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpGet("GetOrderById/{userId}")]
        public ResponseDto? GetOrderById(string userId, int pageSize = 4, int pageNumber = 1)
        {
            try
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
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public ResponseDto? Get(int id)
        {
            try
            {
                OrderHead orderHeader = _db.orderHeads.Include(u => u.OrderDetail).First(u => u.OrderHeadId == id);
                _response.Result = _mapper.Map<OrderHeadDto>(orderHeader);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartsDto cartdto)
        {
            try
            {
                var userId = cartdto.CartHead.UserId;
                var userEmail = cartdto.CartHead.Email;
                OrderHeadDto orderHeaderDto = _mapper.Map<OrderHeadDto>(cartdto.CartHead);
                orderHeaderDto.Status = SD.Status_Pending; //initial status
                orderHeaderDto.OrderDetail = _mapper.Map<IEnumerable<OrderDetailDto>>(cartdto.CartDetail);

                //create order
                OrderHead orderCreated = _db.orderHeads.Add(_mapper.Map<OrderHead>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();
                orderHeaderDto.OrderHeadId = orderCreated.OrderHeadId;
                _response.Result = orderHeaderDto;
                await _cartService.ClearCart(userId);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string newStatus){
            try
            {
                OrderHead orderHeader = _db.orderHeads.Include(o => o.OrderDetail).FirstOrDefault(u => u.OrderHeadId == orderId);
                if (orderHeader != null)
                {
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
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
            }
            return _response;
        }
    }
}