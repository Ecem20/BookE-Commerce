using System.ComponentModel.DataAnnotations;

namespace MVCWeb.Models
{
    public class LoginRequestDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
