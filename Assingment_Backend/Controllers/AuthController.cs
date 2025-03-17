using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Assignment_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService authService;


        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(RegisterModel register)
        {
            var result = await authService.Register(register);

            return result.Succeeded ? Ok(result) : BadRequest(result);

        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState.ToList());

            var result = await authService.Login(login);

            if (!result.SignInResult.Succeeded)
                return Unauthorized(new { Message = result.Message });

            return Ok(result);
        }


        [HttpPost]
        [Route("Admin/Login")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginDTO login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.ToList());

            var result = await authService.Login(login);

            if (!result.SignInResult.Succeeded)
                return Unauthorized(new { Message = result.Message });

            if (result.Role != "Admin")
                return StatusCode(StatusCodes.Status403Forbidden , new { Message = "Bạn không có quyền truy cập." });


            return Ok(result);
        }



        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //[Route("Logout")]
        //public  async Task<IActionResult> Logout()
        //{
        //    await authService.Logout(User);
        //    return Ok();
        //}

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("getuser")]
        public IActionResult GETUSERid()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Ok(userId);
        }


    }   
}
