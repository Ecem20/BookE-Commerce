using FluentValidation;
using SellBooks.Models.DTO;
using AuthAPI.Data;
using System.Text.RegularExpressions;

namespace SellBooksAuthService.FluentValidation
{
    public class RegistrationValidator : AbstractValidator<RegistrationRequestDto>
    {

        private readonly ApplicationDbContext _db;

        public RegistrationValidator(ApplicationDbContext db)
        {
            _db = db;

            RuleFor(user => user.Name)
             .NotEmpty().WithMessage("Enter a name");

            RuleFor(user => user.SurName)
                .NotEmpty().WithMessage("Enter a surname");

            RuleFor(user => user.Email)
                .NotEmpty().WithMessage("Enter an email")
                 .EmailAddress().WithMessage("Invalid email address.")
                 .Must(BeUniqueEmail).WithMessage("This email is already registered");

            RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Enter password")
            .Length(8, 16).WithMessage("Password must be between 8 and 16 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(user => user.Password).Must(password => password.Any(char.IsDigit)).WithMessage("Password must contain at least one digit.");
            RuleFor(user => user.PhoneNumber).NotEmpty().WithMessage("Telefon numarası kısmı boş olamaz.");
            RuleFor(user => user.PhoneNumber).Must(PhoneCheck).WithMessage("Geçerli bir telefon numarası giriniz.");

            RuleFor(user => user.PhoneNumber).Must(UniquePhone).WithMessage("Bu telefon numarası kullanılmaktadır.");


            RuleFor(user => user.BirthDate).NotEmpty().WithMessage("Enter a Birthdate").Must(BeAtLeast18YearsOld).WithMessage("You must be at least 18 years old.");
        }
        private bool BeUniqueEmail(string email)
        {
            var existingemail = _db.ApplicationUsers.FirstOrDefault(b => b.Email == email);
            return existingemail == null;
        }

        private bool UniquePhone(string phone)
        {
            var existingphone = _db.ApplicationUsers.FirstOrDefault(b => b.PhoneNumber == phone);
            return existingphone == null;
        }

        private bool BeAtLeast18YearsOld(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;
            if (birthDate.Date > today.AddYears(-age))
                age--;
            return age >= 18;
        }

        private bool PhoneCheck(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^[1-9]\d{9}$");
        }

    }
}
