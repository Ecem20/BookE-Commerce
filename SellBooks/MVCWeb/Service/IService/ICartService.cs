using MVCWeb.Models;

namespace MVCWeb.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto?> GetCartByUserIdAsync(string userId);
        Task<ResponseDto?> UpsertCartAsync(CartsDto cartDto);
        Task<ResponseDto?> RemoveFromCartAsync(int CartDetailsId);

        Task<ResponseDto?> RemoveCartItemAsync(int cartDetailId, int countToRemove);
        Task<ResponseDto?> AddCartItemAsync(int cartDetailId, int countToAdd);


        Task<ResponseDto?> ClearCartAsync(int CartHeaderId);

    }
}
