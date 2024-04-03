namespace MVCWeb.Models
{
    public class LoginResponseDto
    {
        public UserDto User { get; set; }  //user will have the details of the login 
        public string Token { get; set; } //validate the identity of the user

    }
}
