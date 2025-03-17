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
        private  readonly ICartService _cartService;
        private  readonly IOrderService _orderService;

        public OrderController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;
            _orderService = orderService;
        }

        [HttpGet("confirm")]
        public async Task<IActionResult> Process()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _cartService.GetCart(userId);

            return Ok(cart);

        }

        [HttpPost]
        public async Task<IActionResult> OrderPlace(OrderDTO order)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var newOrder = new Order
            {
                UserId = userId,
                PayMethod = order.PayMethod,
                Address = order.Address,
                OrderDetails = order.OrderDetails,
            }; 

            var OrderResult = await _orderService.PlaceOrderAsync(newOrder);


            return OrderResult.Isuccess ? Ok(OrderResult) : BadRequest(OrderResult);
        }

        [HttpGet("")]
        public  async Task<IActionResult> GetOrdersByUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var orders = await _orderService.GetOrdersByUserIdAsync(userId);

            return orders == null ? BadRequest(orders) : Ok(orders);
        } 

    }
}
