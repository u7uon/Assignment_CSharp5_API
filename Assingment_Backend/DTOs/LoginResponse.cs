using Microsoft.AspNetCore.Identity;

namespace Assignment_Backend.DTOs
{
    public class LoginResponse
    {
        public SignInResult SignInResult { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }

        public string Role { get; set; }


    }
}
