using MVCWeb.Models;

namespace MVCWeb.Service.IService
{
    public interface IAutService
    {
        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDto?> ConfirmEmail(string email);
        Task<ResponseDto?> GetCustomers(int pageSize, int pageNumber);
    }
}
