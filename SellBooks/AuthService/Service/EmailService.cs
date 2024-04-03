using SellBooks.Models.DTO;
using SellBooksAuthService.Service.IService;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace SellBooksAuthService.Service
{
    public class EmailService :IEmailService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EmailService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendEmail(EmailDto emailDto)
        {
            var client = _httpClientFactory.CreateClient("Email");
            var content = new StringContent(JsonConvert.SerializeObject(emailDto), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"/api/email/send", content);
        }
    }
}
