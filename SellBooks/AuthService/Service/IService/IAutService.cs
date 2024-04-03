using Microsoft.AspNetCore.Identity;
using SellBooks.Models;
using SellBooks.Models.DTO;

namespace SellBooks.Service.IService
{
    public interface IAutService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);

    }
}

