using Newtonsoft.Json;
using OrdersAPI.Service.IService;
using System.Text;

namespace OrdersAPI.Service
{
    public class CartService : ICartService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CartService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task ClearCart(string userId)
        {
            var client = _httpClientFactory.CreateClient("Cart");
            var content = new StringContent(JsonConvert.SerializeObject(userId), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/cart/ClearCart/{userId}",content);
        }

    }
}
