namespace OrdersAPI.Service.IService
{
    public interface ICartService
    {
        Task ClearCart(string userId);
    }
}
