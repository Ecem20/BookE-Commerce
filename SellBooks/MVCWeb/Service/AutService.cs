using MVCWeb.Models;
using MVCWeb.Service.IService;
using MVCWeb.Utility;

namespace MVCWeb.Service
{
    public class AutService : IAutService
    {

        private readonly IBaseService _baseService;
        public AutService(IBaseService baseService)
        {
            _baseService = baseService;
        }


        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/AssignRole"
            });
        }
        public async Task<ResponseDto?> ConfirmEmail(string email)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.AuthAPIBase}/api/auth/confirm?email={email}"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/login"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/register"
            }, withBearer: false);
        }

        public async Task<ResponseDto?> GetCustomers(int pageSize, int pageNumber)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.AuthAPIBase + $"/api/auth/userinfo?pageSize={pageSize}&pageNumber={pageNumber}"
            });
        }

    }
}
