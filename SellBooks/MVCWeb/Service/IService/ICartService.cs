using MVCWeb.Models;

namespace MVCWeb.Service.IService
{
    public interface ICartService
    {
        Task<ResponseDto?> GetCartByUserIdAsync(string userId);
        Task<ResponseDto?> UpsertCartAsync(CartsDto cartDto);
        Task<ResponseDto?> RemoveFromCartAsync(int CartDetailsId);
        Task<ResponseDto?> ClearCartAsync(int CartHeaderId);

    }
}
