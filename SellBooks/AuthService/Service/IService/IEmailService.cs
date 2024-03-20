using SellBooks.Models.DTO;

namespace SellBooksAuthService.Service.IService
{
    public interface IEmailService
    {
        Task SendEmail(EmailDto emailDto);
    }
}
