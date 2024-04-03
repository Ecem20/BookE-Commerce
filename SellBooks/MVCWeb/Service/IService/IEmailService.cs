using MVCWeb.Models;

namespace MVCWeb.Service.IService
{
    public interface IEmailService
    {
        Task<ResponseDto?> SendEmail(EmailDto emailDto);
    }
}
