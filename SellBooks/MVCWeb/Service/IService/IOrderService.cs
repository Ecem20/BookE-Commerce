using MVCWeb.Models;

namespace MVCWeb.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseDto?> CreateOrder(CartsDto cartDto);
        Task<ResponseDto?> GetOrders(string? userId, int pageSize, int pageNumber);
        Task<ResponseDto>? GetOrder(int orderId);
        Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus);
    }
}
