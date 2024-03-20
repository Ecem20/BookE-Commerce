 using MVCWeb.Models;
using MVCWeb.Service.IService;
using MVCWeb.Utility;

namespace MVCWeb.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseDto?> CreateOrder(CartsDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto() //endpoint
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.OrderAPIBase + "/api/order/CreateOrder",
            });
        }

        public async Task<ResponseDto?> GetAllOrder(string? userId, int pageSize, int pageNumber)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.OrderAPIBase}/api/order/GetOrders/{userId}?pageSize={pageSize}&pageNumber={pageNumber}"

            });
        }

        public async Task<ResponseDto?> GetOrderByUserId(string userId, int pageSize, int pageNumber)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.OrderAPIBase}/api/order/GetOrderById/{userId}?pageSize={pageSize}&pageNumber={pageNumber}"
            });
        }

        public async Task<ResponseDto>? GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.OrderAPIBase + "/api/order/GetOrder/" + orderId
            });
        }

        public async Task<ResponseDto?> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = newStatus,
                Url = SD.OrderAPIBase + "/api/order/UpdateOrderStatus/" + orderId
            });
        }

    }
}