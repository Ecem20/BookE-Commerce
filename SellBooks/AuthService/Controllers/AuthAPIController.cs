using SellBooks.Models.DTO;
using SellBooks.Service.IService;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using SellBooks.Models;
using SellBooksAuthService.Service.IService;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace SellBooks.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAutService _authService;
        protected ResponseDto _response;
        private readonly UserManager<ApplicationUser> _userManager;


        public AuthAPIController(IAutService authService, UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _response = new();
            _userManager = userManager;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage)) //error occured
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
            return Ok(_response);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {

            var loginResponse = await _authService.Login(model);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = loginResponse.Message;
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error encountered";
                return BadRequest(_response);
            }
            return Ok(_response);

        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    user.UserStatus = "Aktif";
                    var updateResult = await _userManager.UpdateAsync(user);
                    return Ok(new { IsSuccess = true });
                }
            }
            return BadRequest(new { IsSuccess = false });
        }


        [HttpGet("userinfo")]
        public async Task<ResponseDto> GetUserInfo(int pageSize = 4, int pageNumber = 1)
        {
            ResponseDto response = new ResponseDto();

            try
            {
                var customers = await _userManager.GetUsersInRoleAsync("Customer");
                if (customers != null && customers.Any())
                {
                    var customerInfos = customers.Select(customer => new UserDto
                    {
                        Name = customer.Name,
                        SurName = customer.SurName,
                        Email = customer.Email,
                        PhoneNumber = customer.PhoneNumber
                    }).ToList();

                    int totalItems = customerInfos.Count;
                    int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

                    if (pageSize > 0)
                    {
                        customerInfos = customerInfos.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();
                    }

                    response.IsSuccess = true;
                    response.Result = customerInfos;
                    response.TotalPages = totalPages;
                }
                else
                {
                    response.IsSuccess = false;
                    response.Message = "Customer bulunamadı.";
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = "Bir hata oluştu: " + ex.Message;
            }

            return response;
        }

    }
}
