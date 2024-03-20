 using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MVCWeb.Utility;
using Newtonsoft.Json;
using System.Security.Claims;
using MVCWeb.Service.IService;
using MVCWeb.Models;
using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace MVCWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAutService _autService;
        private readonly ITokenProvider _tokenProvider;
        private readonly IEmailService _emailService;

        public AuthController(IAutService autService,ITokenProvider tokenProvider, IEmailService emailService)
        {
            _autService = autService;
            _tokenProvider = tokenProvider;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto obj)
        {
            ResponseDto responseDto = await _autService.LoginAsync(obj);

            if (ModelState.IsValid && responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                await SignInUser(loginResponseDto);
                _tokenProvider.SetToken(loginResponseDto.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                    ViewData["CustomError"] = responseDto.Message;
                    return View(obj);
            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };

            ViewBag.RoleList = roleList;
            return View();        

    }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto obj)
        {
            ResponseDto result = await _autService.RegisterAsync(obj);
            ResponseDto assingRole;

            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(obj.Role))
                {
                    obj.Role = SD.RoleCustomer;
                }
                assingRole = await _autService.AssignRoleAsync(obj);
                if (assingRole != null && assingRole.IsSuccess)
                {
                    // Kullanıcı kaydı başarılıysa e-posta doğrulama linkini oluştur
                    var confirmationLink = Url.Action("ConfirmEmail", "Auth", new {obj.Email}, Request.Scheme);

                // E-posta içeriğini oluştur ve gönder
                var emailDto = new EmailDto
                    {
                        ReceiverEmail = obj.Email,
                        Subject = "Mail Aktivasyonu",
                        Body = $"Hesabınızı doğrulamak için tıklayınız: {confirmationLink}"
                };
                    await _emailService.SendEmail(emailDto);
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                ViewData["CustomError"] = result.Message;
                ModelState.AddModelError("CustomError", result.Message);

            }
            var roleList = new List<SelectListItem>()
            {
                new SelectListItem{Text=SD.RoleAdmin,Value=SD.RoleAdmin},
                new SelectListItem{Text=SD.RoleCustomer,Value=SD.RoleCustomer},
            };

            ViewBag.RoleList = roleList;
            return View(obj);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string email)
        {
            if (!string.IsNullOrEmpty(email)){

                var result = await _autService.ConfirmEmail(email);
                if (result.IsSuccess)
                {
                    ViewBag.IsSuccess = true;
                    return View("ConfirmEmail");
                }
            }
            return View("Error");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        private async Task SignInUser(LoginResponseDto model)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(model.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name,jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(ClaimTypes.Role,jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

    }
}
