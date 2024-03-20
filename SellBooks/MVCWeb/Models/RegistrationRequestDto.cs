using System.ComponentModel.DataAnnotations;

namespace MVCWeb.Models
{
    public class RegistrationRequestDto
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Role { get; set; }
    }
}
