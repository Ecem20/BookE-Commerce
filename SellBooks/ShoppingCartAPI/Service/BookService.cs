using Newtonsoft.Json;
using ShoppingCartAPI.Models.Dto;
using ShoppingCartAPI.Service.IService;
using System.Text;

namespace ShoppingCartAPI.Service
{
    public class BookService : IBookService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BookService(IHttpClientFactory httpClientFactory)
        {
                _httpClientFactory = httpClientFactory;
        }

        public async Task<ProductDto> GetBook(int bookId)
        {
            var client = _httpClientFactory.CreateClient("Book");
            var response = await client.GetAsync($"/api/book/{bookId}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(resp.Result));
            }
            return null;
        }

        public async Task<IEnumerable<ProductDto>> GetBooks()
        {
            var client = _httpClientFactory.CreateClient("Book");
            var response = await client.GetAsync($"/api/book/getbooks");
            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(resp.Result));
            }
            return new List<ProductDto>();
        }

        public async Task UpdateBook(ProductDto book)
        {
            var client = _httpClientFactory.CreateClient("Book");
            var content = new StringContent(JsonConvert.SerializeObject(book), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/book/Update", content);
        }
    }
}