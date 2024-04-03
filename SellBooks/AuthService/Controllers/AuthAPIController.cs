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
using AuthAPI.Data;

namespace SellBooks.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAutService _authService;
        protected ResponseDto _response;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;


        public AuthAPIController(IAutService authService, UserManager<ApplicationUser> userManager,ApplicationDbContext db)
        {
            _authService = authService;
            _response = new();
            _userManager = userManager;
            _db = db;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var errorMessage = await _authService.Register(model);
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
                throw new Exception("Hata oluştu");
            }
            return Ok(_response);
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmEmailAsync(string email) //üye olan kullanıcının mailini confirm etme
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
            throw new Exception("Mail bulunamadı");
        }


        [HttpGet("userinfo")]
        public async Task<ActionResult<ResponseDto>> GetUserInfo(int pageSize = 4, int pageNumber = 1)
        {
            if (pageSize <= 0 || pageNumber <= 0 || pageSize == null || pageNumber == null)
            {
                throw new Exception("Page size or page number is less than or equal to zero");
            }

            ResponseDto response = new ResponseDto();

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

                return Ok(response);
            }
            else
            {
                throw new Exception("Kullanıcı bulunamadı");
            }
        }

        [HttpGet("user")]
        public async Task<ActionResult<ResponseDto>> GetUser(string userId)
        {
            ResponseDto response = new ResponseDto();

            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var userDto = new UserDto
                {
                    Name = user.Name,
                    SurName = user.SurName,
                    PhoneNumber = user.PhoneNumber
                };

                response.IsSuccess = true;
                response.Result = userDto;
                return Ok(response);
            }
            else
            {
                throw new Exception("Kullanıcı bulunamadı");
            }
        }

        [HttpGet("checkuser")]
        public async Task<IActionResult> CheckUser(string userId) //sepete ürün eklerken öyle bir user ıd veritabanında var mı yok mu
        {
            var user = await _userManager.FindByIdAsync(userId);
            return Ok(user);
        }

        [HttpGet("checkmail")]
        public async Task<IActionResult> CheckMail(string userId,string email) //sepete ürün eklerken ki girilen mail, girilen user id ye ait mi değil mi
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user.Email != email)
            {
                return Ok(null);          
            }
            return Ok(user);
        }

        [HttpGet("Email")]
        public ResponseDto EmailCheck(string email) //check email is unique or not
        {
            ResponseDto response = new ResponseDto();
            var existingemail = _db.ApplicationUsers.FirstOrDefault(b => b.Email == email);
            if (existingemail == null)
            {
                response.IsSuccess = true;
            }
            else
            {
                response.IsSuccess = false;
            }
            return response;
        }

        [HttpGet("Phone")]
        public ResponseDto PhoneCheck(string phone) // phone is unique or not
        {
            ResponseDto response = new ResponseDto();
            var existingphone = _db.ApplicationUsers.FirstOrDefault(b => b.PhoneNumber == phone);
            if (existingphone == null)
            {
                response.IsSuccess = true;
            }
            else
            {
                response.IsSuccess = false;
            }
            return response;
        }


        [HttpGet("role")]
        public async Task<ResponseDto> Role(string userId)
        {
            ResponseDto response = new ResponseDto();
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("ADMIN"))
            {
                response.IsSuccess = true;
            }
            else
            {
                response.IsSuccess = false;
            }
            return response;
        }
    }
}