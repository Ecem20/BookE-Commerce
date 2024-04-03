using MVCWeb.Models;
using MVCWeb.Service.IService;
using MVCWeb.Utility;
using System.Drawing.Printing;

namespace MVCWeb.Service
{
    public class CartService : ICartService
    {

        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> ClearCartAsync(int CartHeaderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = CartHeaderId,
                Url = SD.ShoppingCartAPIBase + "/api/cart/ClearCart"
            });
        }

        public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.ShoppingCartAPIBase + "/api/cart/GetCart/" + userId
            });
        }

        public async Task<ResponseDto?> RemoveFromCartAsync(int CartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = CartDetailsId,
                Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCart"
            });
        }

        public async Task<ResponseDto?> RemoveCartItemAsync(int cartDetailId, int countToRemove)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.ShoppingCartAPIBase + $"/api/cart/RemoveCartItem?cartDetailId={cartDetailId}&countToRemove={countToRemove}"
            });
        }

        public async Task<ResponseDto?> AddCartItemAsync(int cartDetailId, int countToAdd)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Url = SD.ShoppingCartAPIBase + $"/api/cart/AddCartItem?cartDetailId={cartDetailId}&countToAdd={countToAdd}"
            });
        }

        public async Task<ResponseDto?> UpsertCartAsync(CartsDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/CartUpsert"
            });
        }
    }
}