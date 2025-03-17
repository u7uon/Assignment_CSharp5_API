using Assignment_Backend.DTOs;
using Assignment_Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Assignment_Backend.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginDTO login);

        Task Logout(User user);

        Task<IdentityResult> Register(RegisterModel reg);

        Task<IdentityResult> RefeshLogin();

        //Task<IdentityResult> ResetPassword(ResetPasswordDTO reset);  
    }
}
