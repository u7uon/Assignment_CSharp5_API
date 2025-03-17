using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Assignment_Backend.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpPost]
        [Route("Add")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddToCart(AddToCartDTO item)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized(new { message = "chưa đăng nhập" });


            var result = await _cartService.AddToCart(userId, item.ProductId, item.quantity);

            return result.Isuccess ? Ok(result) : BadRequest(result);

        }


        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUserCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("Chưa đăng nhập");

            var cart = await _cartService.GetCart(userId);
            return Ok(cart);

        }

        [HttpPut("{itemId}/increase")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> IncreaseItem(int itemId)
        {
            if (itemId == 0)
                return BadRequest("Dữ liệu không hợp lệ");

            var result = await _cartService.InscreaseItemByIdAsync(itemId);

            return result.Isuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPut("{itemId}/decrease")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> DecreaseItem(int itemId)
        {
            if (itemId == 0)
                return BadRequest("Dữ liệu không hợp lệ");

            var result = await _cartService.DescreaseItemByIdAsync(itemId);

            return result.Isuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{itemId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme )]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            var result = await _cartService.RemoveCartItem(itemId);

            return result.Isuccess ? Ok(result) : BadRequest(result);
        }





    }
}
