namespace SellBooks.Models.DTO
{
    public class LoginResponseDto
    {
        public UserDto User { get; set; }  //user will have the details of the login 
        public string Token { get; set; } //validate the identity of the user
        public string Message { get; set; }
    }
}