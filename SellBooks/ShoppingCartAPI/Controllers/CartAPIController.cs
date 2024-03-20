using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCartAPI.Data;
using ShoppingCartAPI.Models;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Service.IService;

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
        public CartAPIController(ApplicationDbContext db, IMapper mapper, IBookService bookService)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
            _bookService = bookService;
        }


        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartsDto cart = new()
                {
                    CartHead = _mapper.Map<CartHeadDto>(_db.cartHeads.First(u => u.UserId == userId))
                };
                cart.CartDetail = _mapper.Map<IEnumerable<CartDetailDto>>(_db.cartDetail
                    .Where(u => u.CartHeadId == cart.CartHead.CartHeadId));

                IEnumerable<ProductDto> books = await _bookService.GetBooks();
                foreach (var item in cart.CartDetail)
                {
                    item.Product = books.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cart.CartHead.CartTotal += Convert.ToDouble(item.Count * item.Product.ProductPrice);
                }
                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartsDto cartsDTO)
        {
            try
            {
                var newlyAddedQuantity = cartsDTO.CartDetail.First().Count;
                var book = await _bookService.GetBook(cartsDTO.CartDetail.First().ProductId);

                if (book.ProductStockQuantity < cartsDTO.CartDetail.First().Count)
                {
                    throw new Exception($"Satın almak istediğiniz üründen stoğumuzda yalnızca {book.ProductStockQuantity} adet vardır.");
                }

                var cartHeaderFromDb = await _db.cartHeads.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartsDTO.CartHead.UserId);
                if (cartHeaderFromDb == null) //sepet boşsa
                {
                    //create header and details
                    CartHead cartHeader = _mapper.Map<CartHead>(cartsDTO.CartHead);
                    _db.cartHeads.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    // Now create details
                    foreach (var cartDetailDTO in cartsDTO.CartDetail)
                    {
                        // Explicitly set the CartHeaderId
                        cartDetailDTO.CartHeadId = cartHeader.CartHeadId;
                        _db.cartDetail.Add(_mapper.Map<CartDetail>(cartDetailDTO));
                    }
                    await _db.SaveChangesAsync();
                }
                else //sepet doluysa
                {
                    //check if details has same product
                    var cartDetailsFromDb = await _db.cartDetail.AsNoTracking().FirstOrDefaultAsync(u => u.ProductId == cartsDTO.CartDetail.First().ProductId && u.CartHeadId == cartHeaderFromDb.CartHeadId);
                    if (cartDetailsFromDb == null) //sepette eklenmek istenen ürün yoksa
                    { //create cartdetails
                        cartsDTO.CartDetail.First().CartHeadId = cartHeaderFromDb.CartHeadId;
                        _db.cartDetail.Add(_mapper.Map<CartDetail>(cartsDTO.CartDetail.First()));
                        await _db.SaveChangesAsync();
                    }
                    else //update
                    {
                        cartsDTO.CartDetail.First().Count += cartDetailsFromDb.Count;
                        cartsDTO.CartDetail.First().CartHeadId = cartDetailsFromDb.CartHeadId;
                        cartsDTO.CartDetail.First().CartDetailId = cartDetailsFromDb.CartDetailId;
                        _db.cartDetail.Update(_mapper.Map<CartDetail>(cartsDTO.CartDetail.First()));
                        await _db.SaveChangesAsync();
                    }
                }
                if (book.ProductStockQuantity == 1)
                {
                    book.ProductStockQuantity = 0;
                }
                else
                {
                    book.ProductStockQuantity -= newlyAddedQuantity;

                }
                await _bookService.UpdateBook(book);
                await _db.SaveChangesAsync();
                _response.Result = cartsDTO;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int CartDetailId)
        {
            try
            {
                CartDetail cartDetails = await _db.cartDetail.FirstOrDefaultAsync(u => u.CartDetailId == CartDetailId);

                if (cartDetails != null)
                {
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
                }
                else
                {
                    _response.Result = false;
                    _response.Message = "Cart item not found.";
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("ClearCart/{userId}")]
        public async Task<ResponseDto> ClearCart(string userId)
        {
            try
            {
                // Get cart header of the user 
                var cartHeader = await _db.cartHeads.FirstOrDefaultAsync(u => u.UserId == userId);
                if (cartHeader != null)
                {
                    // Get all cart details associated with the cart header
                    var cartDetails = await _db.cartDetail.Where(cd => cd.CartHeadId == cartHeader.CartHeadId).ToListAsync();

                    // Remove from cartdetail
                    foreach (var cartDetail in cartDetails)
                    {
                        _db.cartDetail.Remove(cartDetail);
                    }
                    // Remove from cart header
                    _db.cartHeads.Remove(cartHeader);
                    await _db.SaveChangesAsync();
                    _response.Result = true;
                }
                else
                {
                    _response.IsSuccess = false;
                }
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

    }
}