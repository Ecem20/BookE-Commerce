using Microsoft.AspNetCore.Identity;

namespace SellBooks.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string SurName { get; set; }
        public DateTime BirthDate { get; set; }
        public string UserStatus { get; set; } = "Onay Bekliyor";
    }
}
