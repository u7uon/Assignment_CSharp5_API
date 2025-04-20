using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Assignment_Backend.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = "User")]
    public class OrderController : Controller
    {
        private  readonly IOrderService _orderService;

        public OrderController(ICartService cartService, IOrderService orderService)
        {

            _orderService = orderService;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> OrderPlace([FromBody]OrderDTO order)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized(new { Message = "Chưa đăng nhập" });

            var OrderResult = await _orderService.PlaceOrderAsync(userId, order);


            return OrderResult.Isuccess ? Ok(new { Message = OrderResult.Message }) : BadRequest(new {Message = OrderResult.Message});
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "User")]
        public  async Task<IActionResult> GetOrdersByUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

           var orders = await _orderService.GetOrdersByUserIdAsync(userId);

            return orders == null ? BadRequest(orders) : Ok(orders);
        } 

    }
}
