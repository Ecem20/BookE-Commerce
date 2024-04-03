using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Service.IService;
using System.Net.Http;
using System.Net;
using System.Text;

namespace ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        private IBookService _bookService;
        private readonly IHttpClientFactory _httpClientFactory;

        public CartAPIController(ApplicationDbContext db, IMapper mapper, IBookService bookService, IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
            _bookService = bookService;
            _httpClientFactory = httpClientFactory;
        }


        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
                CartsDto cart = new()
                {
                    CartHead = _mapper.Map<CartHeadDto>(_db.cartHeads.First(u => u.UserId == userId))
                };
                cart.CartDetail = _mapper.Map<IEnumerable<CartDetailDto>>(_db.cartDetail.Where(u => u.CartHeadId == cart.CartHead.CartHeadId));

                foreach (var item in cart.CartDetail)
                {
                    item.Product = await _bookService.GetBook(item.ProductId);
                    cart.CartHead.CartTotal += Convert.ToDouble(item.Count * item.Product.ProductPrice);
                }
                _response.Result = cart;
            return _response;
        }


        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartsDto cartsDTO)
        {
            //error handling
            var client = _httpClientFactory.CreateClient("Auth");
            var content = new StringContent(JsonConvert.SerializeObject(cartsDTO.CartHead.UserId), Encoding.UTF8, "application/json");
            var response = await client.GetAsync($"/api/auth/checkuser?userId={cartsDTO.CartHead.UserId}");

            if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception($"Could not find {cartsDTO.CartHead.UserId}");
            }

            //error handling
            var res = await client.GetAsync($"/api/auth/checkmail?userId={cartsDTO.CartHead.UserId}&email={cartsDTO.CartHead.Email}");

            if (res.StatusCode == HttpStatusCode.NoContent || res.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new Exception($"Could not find {cartsDTO.CartHead.Email}");
            }

            var newlyAddedQuantity = cartsDTO.CartDetail.First().Count;
            var book = await _bookService.GetBook(cartsDTO.CartDetail.First().ProductId);

            if (book.ProductStockQuantity < cartsDTO.CartDetail.First().Count)
            {
                throw new Exception($"Satın almak istediğiniz üründen stoğumuzda yalnızca {book.ProductStockQuantity} adet vardır.");
            }

            var cartHeaderFromDb = await _db.cartHeads.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartsDTO.CartHead.UserId);
            if (cartHeaderFromDb == null)
            {
                CartHead cartHeader = _mapper.Map<CartHead>(cartsDTO.CartHead);
                _db.cartHeads.Add(cartHeader);
                await _db.SaveChangesAsync();

                foreach (var cartDetailDTO in cartsDTO.CartDetail)
                {
                    cartDetailDTO.CartHeadId = cartHeader.CartHeadId;
                    _db.cartDetail.Add(_mapper.Map<CartDetail>(cartDetailDTO));
                }
                await _db.SaveChangesAsync();
            }
            else
            {
                var cartDetailsFromDb = await _db.cartDetail.AsNoTracking().FirstOrDefaultAsync(u => u.ProductId == cartsDTO.CartDetail.First().ProductId && u.CartHeadId == cartHeaderFromDb.CartHeadId);
                if (cartDetailsFromDb == null)
                {
                    cartsDTO.CartDetail.First().CartHeadId = cartHeaderFromDb.CartHeadId;
                    _db.cartDetail.Add(_mapper.Map<CartDetail>(cartsDTO.CartDetail.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    cartsDTO.CartDetail.First().Count += cartDetailsFromDb.Count;
                    cartsDTO.CartDetail.First().CartHeadId = cartDetailsFromDb.CartHeadId;
                    cartsDTO.CartDetail.First().CartDetailId = cartDetailsFromDb.CartDetailId;
                    _db.cartDetail.Update(_mapper.Map<CartDetail>(cartsDTO.CartDetail.First()));
                    await _db.SaveChangesAsync();
                }
            }

            book.ProductStockQuantity -= newlyAddedQuantity;

            await _bookService.UpdateBook(book);
            await _db.SaveChangesAsync();
            _response.Result = cartsDTO;

            return _response;

        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int CartDetailId)
        {
            CartDetail cartDetails = await _db.cartDetail.FirstOrDefaultAsync(u => u.CartDetailId == CartDetailId);

            // Get the book using BookService
            var book = await _bookService.GetBook(cartDetails.ProductId);

            if (book != null)
            {
                // Increase stock quantity
                book.ProductStockQuantity += cartDetails.Count;
                // Update the book in the database
                await _bookService.UpdateBook(book);
            }

            int totalCountofCartItem = _db.cartDetail.Count(u => u.CartHeadId == cartDetails.CartHeadId);
            _db.cartDetail.Remove(cartDetails);

            if (totalCountofCartItem == 1) // Last item is being removed from the shopping cart
            {
                var cartHeadertoRemove = await _db.cartHeads.FirstOrDefaultAsync(u => u.CartHeadId == cartDetails.CartHeadId);
                _db.cartHeads.Remove(cartHeadertoRemove);
            }
            await _db.SaveChangesAsync();

            _response.Result = true;
            return _response;
        }


        [HttpPost("RemoveCartItem")]
        public async Task<ResponseDto> RemoveCartItem(int cartDetailId, int countToRemove)
        {
            CartDetail cartDetails = await _db.cartDetail.FirstOrDefaultAsync(u => u.CartDetailId == cartDetailId);
            var book = await _bookService.GetBook(cartDetails.ProductId);
            if (countToRemove >= cartDetails.Count)
            {
                _db.cartDetail.Remove(cartDetails);
            }
            else
            {
                cartDetails.Count -= countToRemove;
            }
            book.ProductStockQuantity += countToRemove;
            await _bookService.UpdateBook(book);
            await _db.SaveChangesAsync();
            _response.Result = true;
            return _response;
        }


        [HttpPost("AddCartItem")]
        public async Task<ResponseDto> AddCartItem(int cartDetailId, int countToAdd)
        {
            CartDetail cartDetails = await _db.cartDetail.FirstOrDefaultAsync(u => u.CartDetailId == cartDetailId);
            var book = await _bookService.GetBook(cartDetails.ProductId);
            cartDetails.Count += countToAdd;
            book.ProductStockQuantity -= countToAdd;
            await _bookService.UpdateBook(book);
            await _db.SaveChangesAsync();
            _response.Result = true;
            return _response;
        }


        [HttpPost("ClearCart/{userId}")]
        public async Task<ResponseDto> ClearCart(string userId)
        {
                // Get cart header of the user 
                var cartHeader = await _db.cartHeads.FirstOrDefaultAsync(u => u.UserId == userId);
                // Get all cart details associated with the cart header
                var cartDetails = await _db.cartDetail.Where(cd => cd.CartHeadId == cartHeader.CartHeadId).ToListAsync();

                 foreach (var cartDetail in cartDetails)
                 {
                        _db.cartDetail.Remove(cartDetail);
                 }
                 _db.cartHeads.Remove(cartHeader);
                 await _db.SaveChangesAsync();
                 _response.Result = true;
            return _response;
        }
    }
}