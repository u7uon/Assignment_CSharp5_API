using Assignment_Backend.DTOs;
using Assignment_Backend.Models;
using Assingment_Backend.DTOs;
using Microsoft.AspNetCore.Identity;
using static Assignment_Backend.Controllers.AuthController;

namespace Assignment_Backend.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginDTO login);

        Task Logout(User user);

        Task<IdentityResult> Register(RegisterModel reg);

        Task<IdentityResult> RefeshLogin();

        OAuthProviderConfig GetOAuthProviderConfig(OAuthProvider provider);

        Task<LoginResponse> OAuthLoginAsync(OAuthLoginRequest request); 

        string GenerateJwtToken(User user);

        //Task<IdentityResult> ResetPassword(ResetPasswordDTO reset);  
    }
}
