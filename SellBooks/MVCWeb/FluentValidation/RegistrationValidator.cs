using FluentValidation;
using MVCWeb.Models;
using MVCWeb.Service;
using MVCWeb.Service.IService;
using System.Text.RegularExpressions;


namespace MVCWeb.FluentValidation
{
    public class RegistrationValidator : AbstractValidator<RegistrationRequestDto>
    {
        private readonly IAutService _autService;


        public RegistrationValidator(IAutService autService)
        {
            _autService = autService;

            RuleFor(user => user.Email).EmailAddress().WithMessage("Geçerli bir mail adresi giriniz");
            RuleFor(user => user.Email).NotEmpty().WithMessage("Mail giriniz");
            RuleFor(user => user.Email).Must((email) => BeUniqueEmail(email)).WithMessage("Bu mail kullanılmaktadır");

            RuleFor(user => user.Name).NotEmpty().WithMessage("İsim giriniz");
            RuleFor(user => user.SurName).NotEmpty().WithMessage("Soyisim giriniz");
            RuleFor(user => user.PhoneNumber).NotEmpty().WithMessage("Telefon numarası giriniz");
            RuleFor(user => user.PhoneNumber).Must(PhoneCheck).WithMessage("Geçerli bir telefon numarası giriniz");
            RuleFor(user => user.PhoneNumber).Must((phone) => UniquePhone(phone)).WithMessage("Bu telefon numarası kullanılmaktadır.");

            RuleFor(user => user.Password).NotEmpty().WithMessage("Şifre giriniz");
            RuleFor(user => user.BirthDate).NotEmpty().WithMessage("Doğum tarihi giriniz");
            RuleFor(user => user.BirthDate).Must(BeAtLeast18YearsOld).WithMessage("Üye olabilmek için en az 18 yaşında olmalısınız");


            RuleFor(user => user.Password).Length(8, 16).WithMessage("Şifre minimum 8 maksimum 16 karakterde olmalıdır");
            RuleFor(user => user.Password).Must(password => password.Any(char.IsLower)).WithMessage("Şifre en az 1 küçük harf içermelidir");
            RuleFor(user => user.Password).Must(password => password.Any(char.IsUpper)).WithMessage("Şifre en az 1 büyük harf içermelidir");
            RuleFor(user => user.Password).Must(password => password.Any(char.IsDigit)).WithMessage("Şifre en az 1 sayı içermelidir");
            RuleFor(user => user.Password).Must(password => password.Any(ch => !char.IsLetterOrDigit(ch))).WithMessage("Şifre en az 1 özel karakter içermelidir");
        }


        public bool BeAtLeast18YearsOld(DateTime birthDate)
        {
            return birthDate <= DateTime.Now.AddYears(-18) ? true : false;
        }

        public bool PhoneCheck(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^[1-9]\d{9}$");
        }
        public bool BeUniqueEmail(string email)
        {
            var existingmail = _autService.GetMail(email).Result;
            if (existingmail.IsSuccess == false)
            {
                return existingmail == null;
            }
            return true;

        }

        public bool UniquePhone(string phone)
        {
            var existingphone = _autService.GetPhone(phone).Result;
            if (existingphone.IsSuccess == false)
            {
                return existingphone == null;
            }
            return true;
        }
    }
}
