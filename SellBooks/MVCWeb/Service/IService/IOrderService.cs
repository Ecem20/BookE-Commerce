using MVCWeb.Models;

namespace MVCWeb.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartsDto cartDto);
        Task<ResponseDto?> GetAllOrder(string? userId, int pageSize, int pageNumber);
        Task<ResponseDto?> GetOrderByUserId(string userId,int pageSize,int pageNumber);
        Task<ResponseDto>? GetOrder(int orderId);
        Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus);
    }
}
