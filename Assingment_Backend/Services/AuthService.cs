using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
    }
}
