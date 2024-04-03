using AuthAPI.Data;
using SellBooks.Models;
using SellBooks.Models.DTO;
using SellBooks.Service.IService;
using Microsoft.AspNetCore.Identity;
using System;
using FluentValidation;
using SellBooksAuthService.Service.IService;
using Azure.Core;

namespace SellBooks.Service
{
    public class AutService : IAutService
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;


        public AutService(ApplicationDbContext db,UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            if (user == null)
            {
                return new LoginResponseDto() { User = null, Token = "", Message = "User not found" };
            }

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password); //check password

            // E-posta onaylanmamışsa ve status onay bekliyor ise girişe izin verme
            if (!user.EmailConfirmed || user.UserStatus == "Onay Bekliyor")
            {
                return new LoginResponseDto() { User = null, Token = "", Message = "Your email address is not confirmed yet." };
            }

            if (isValid == false)
            {
                return new LoginResponseDto() { User = null, Token = "" , Message = "Username or password is incorrect." };
            }

            //if user was found , Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user,roles);

            UserDto userDTO = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                SurName = user.SurName,
                BirthDate = user.BirthDate,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDTO,
                Token = token
            };
            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                SurName = registrationRequestDto.SurName,
                BirthDate = registrationRequestDto.BirthDate,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };
            var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
            if (result.Succeeded) //registration is successful
            {
                    await AssignRole(user.Email, "Customer");

                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber,
                        BirthDate = userToReturn.BirthDate,
                        SurName = userToReturn.SurName,
                    };
                    return "";
            }
            else
            {
                return result.Errors.FirstOrDefault().Description;
            }
        }
    }
}