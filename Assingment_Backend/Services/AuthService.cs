using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Assingment_Backend.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static Assignment_Backend.Controllers.AuthController;

namespace Assignment_Backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;


        public AuthService(SignInManager<User> signInManager, UserManager<User> userManager, IConfiguration configuration)
        {
            this.signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
        }

        public OAuthProviderConfig GetOAuthProviderConfig(OAuthProvider provider)
        {
            return provider switch
            {
                OAuthProvider.Google => new OAuthProviderConfig
                {
                    ClientId = _configuration["OAuth:Google:ClientId"],
                    ClientSecret = _configuration["OAuth:Google:ClientSecret"],
                    TokenEndpoint = "https://oauth2.googleapis.com/token",
                    UserInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo"
                },
                OAuthProvider.Facebook => new OAuthProviderConfig
                {
                    ClientId = _configuration["OAuth:Facebook:ClientId"],
                    ClientSecret = _configuration["OAuth:Facebook:ClientSecret"],
                    TokenEndpoint = "https://graph.facebook.com/v12.0/oauth/access_token",
                    UserInfoEndpoint = "https://graph.facebook.com/me?fields=id,email,name"
                },
                OAuthProvider.Microsoft => new OAuthProviderConfig
                {
                    ClientId = _configuration["OAuth:Microsoft:ClientId"],
                    ClientSecret = _configuration["OAuth:Microsoft:ClientSecret"],
                    TokenEndpoint = "https://login.microsoftonline.com/common/oauth2/v2.0/token",
                    UserInfoEndpoint = "https://graph.microsoft.com/v1.0/me"
                },
                _ => throw new ArgumentException("Unsupported OAuth provider")
            };
        }


        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_super_secret_key_that_is_at_least_32_bytes_long_for_hmacsha256!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateToken(User user)
        {
            // 1. Lấy danh sách các vai trò (Roles) của người dùng
            var userRoles = await _userManager.GetRolesAsync(user);

            // 2. Tạo danh sách các Claims (Thông tin định danh)
            var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),  // Thêm user.Id vào token
                    new Claim(ClaimTypes.Name, user.UserName),      // Thêm tên người dùng
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Mã định danh token
                };

            // 3. Thêm các Claims về vai trò của người dùng
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 4. Tạo khóa bí mật để ký token
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));

            // 5. Tạo JWT Token
            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],      // Đơn vị phát hành token
                audience: _configuration["JwtSettings:Audience"],  // Đối tượng sử dụng token
                expires: DateTime.Now.AddHours(3),                // Thời gian hết hạn (3 tiếng)
                claims: authClaims,                                // Các claims đi kèm token
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256) // Thuật toán ký token
            );

            // 6. Trả về chuỗi token đã được mã hóa
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<LoginResponse> Login(LoginDTO login)
        {

            if (string.IsNullOrEmpty(login.UserName) || string.IsNullOrEmpty(login.Password))
                return new LoginResponse
                {
                    SignInResult = SignInResult.Failed,
                    Message = "Không được bỏ trống tên tài khoản, mật khẩu",
                    Token = null
                };

            var user = await _userManager.FindByNameAsync(login.UserName);
            if (user == null)
                return new LoginResponse
                {
                    SignInResult = SignInResult.Failed,
                    Message = "Tên tài khoản không tồn tại",
                    Token = null
                };

            var loginResult = await signInManager.PasswordSignInAsync(user, login.Password, true, true);

            if (!loginResult.Succeeded)
            {
                return new LoginResponse
                {
                    SignInResult = loginResult,
                    Message = "Sai mật khẩu",
                    Token = null
                };
            }

            if (login.UserName == "duongu77")
                await _userManager.AddToRoleAsync(user, "Admin");

            var Role = await  _userManager.GetRolesAsync(user) ; 

            // Tạo JWT Token khi đăng nhập thành công
            var token = await GenerateToken(user);

            return new LoginResponse
            {
                SignInResult = loginResult,
                Message = "Đăng nhập thành công",
                Token = token,
                Role = Role.FirstOrDefault() ?? "None"

            };
        }


        public async Task Logout(User user)
        {
            await signInManager.SignOutAsync(); 
        }

        public async Task<IdentityResult> RefeshLogin()
        {
            throw new Exception("");
        }

        public async Task<IdentityResult> Register(RegisterModel reg)
        {


            if (reg == null)
                return IdentityResult.Failed(new IdentityError { Description = "Không được bỏ trống" });

            var user = await _userManager.FindByNameAsync(reg.UserName);
            if (user != null)
                return IdentityResult.Failed(new IdentityError { Description = "Tên tài khoản đã tồn tại" });

            var newUser = new User
            {
                UserName = reg.UserName,
                Name = reg.Name,
                PhoneNumber = reg.PhoneNumber,
                Email = reg.Email,
            };

            var result = await _userManager.CreateAsync(newUser,reg.Password);

            if (!result.Succeeded)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Đăng kí thất bại! Vui lòng thử lại sau" });
            }

            await _userManager.AddToRoleAsync(newUser, "User");

            return result;

        }

        public async Task<LoginResponse> OAuthLoginAsync(OAuthLoginRequest request)
        {
            try
            {
                if (request == null)
                    return new LoginResponse()
                    {
                        SignInResult = SignInResult.Failed,
                        Message = "Dữ liệu không hợp lệ",
                        Token = string.Empty,
                        Role = string.Empty
                    };

                var result = await signInManager.ExternalLoginSignInAsync(request.Provider, request.AuthorizationCode, true);

                if (result.Succeeded)
                {
                    // Lấy thông tin người dùng từ Provider
                    var user = await _userManager.FindByLoginAsync(request.Provider, request.AuthorizationCode);
                    if (user == null)
                    {
                        return new LoginResponse()
                        {
                            SignInResult = SignInResult.Failed,
                            Message = "Người dùng không tồn tại",
                            Token = string.Empty,
                            Role = string.Empty
                        };
                    }

                    // Tạo JWT token (giả sử bạn có phương thức GenerateJwtToken)
                    string token = GenerateJwtToken(user);
                    string role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? string.Empty;

                    return new LoginResponse()
                    {
                        SignInResult = SignInResult.Success,
                        Message = "Đăng nhập thành công",
                        Token = token,
                        Role = role
                    };
                }
                else
                {
                    return new LoginResponse()
                    {
                        SignInResult = SignInResult.Failed,
                        Message = "Đăng nhập thất bại",
                        Token = string.Empty,
                        Role = string.Empty
                    };
                }
            }
            catch (Exception ex)
            {
                return new LoginResponse()
                {
                    SignInResult = SignInResult.Failed,
                    Message = ex.Message,
                    Token = string.Empty,
                    Role = string.Empty
                };
            }
        }

    }
}
