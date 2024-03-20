using MVCWeb.Models;
using MVCWeb.Service.IService;
using MVCWeb.Utility;

namespace MVCWeb.Service
{
    public class EmailService : IEmailService
    {
        private readonly IBaseService _baseService;
        public EmailService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> SendEmail(EmailDto emailDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = emailDto,
                Url = SD.EmailAPIBase + "/api/email/send",
            });
        }
    }
}
