using FluentValidation;
using MVCWeb.Models;

namespace MVCWeb.FluentValidation
{
    public class RegistrationValidator : AbstractValidator<RegistrationRequestDto>
    {

        public RegistrationValidator()
        {
            RuleFor(user => user.Email).EmailAddress().WithMessage("Invalid email address.");
            //.Must(BeUniqueEmail).WithMessage("This email is already registered.");

            //RuleFor(user => user.Password)
            //   .NotEmpty().WithMessage("Enter a password.")
            //   .Length(8, 16).WithMessage("Password must be between 8 and 16 characters.")
            //   .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            //   .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            //   .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
            //   .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

            RuleFor(user => user.BirthDate).Must(BeAtLeast18YearsOld).WithMessage("You must be at least 18 years old.");
        }

       
        //private bool BeUniqueEmail(string email)
        //{
        //    var existingemail = _db.ApplicationUsers.FirstOrDefault(b => b.Email == email);
        //    return existingemail == null;
        //}

        private bool BeAtLeast18YearsOld(DateTime birthDate)
        {
            return birthDate < DateTime.Now.AddYears(-18);
        }

    }
}
