using Assignment_Backend.Data;
using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design.Internal;

namespace Assignment_Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;
        private readonly ApplicationContext _context;
        private readonly ICartService _cartService;

        public OrderService(IOrderRepository orderRepository, IProductService productService, ApplicationContext context, ICartService cartService)
        {
            _orderRepository = orderRepository;
            _productService = productService;
            _context = context;
            _cartService = cartService;
        }

        public async Task<ServiceResponse> CancelOrderAsync(int OrderId)
        {
            var order = await _orderRepository.GetOrdersByIdAsync(OrderId);
            if (order == null)
                return new ServiceResponse(false, "Đơn hàng không tồn tại");

            if (order.OrderStatus != Order.Status.Pending.ToString())
                return new ServiceResponse(false, "Trạng thái đơn hàng không thể hủy ");

            order.OrderStatus = Order.Status.Canceled.ToString();
            await _orderRepository.SaveChanges();

            return new ServiceResponse(true, "Hủy đơn hàng thành công");

        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetOrdersByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;

            return await _orderRepository.GetOrdersByUserIdAsync(userId);
        }

        public async Task<ServiceResponse> PlaceOrderAsync(string userId, OrderDTO orderdto)
        {

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    //Kiểm tra giỏ
                    var cart = await _cartService.GetCart(userId);

                    if (cart == null || !cart.cartItems.Any())
                        return new ServiceResponse(false, "Giỏ hàng trống");

                    var order = new Order
                    {
                        UserId = userId,
                        OrderDate = DateTime.Now,
                        OrderStatus = Order.Status.Pending.ToString(),
                        Address = orderdto.Address,
                        //PhoneNumber = orderInfo.PhoneNumber,
                        PayMethod = orderdto.PayMethod,
                        TotalAmount = cart.cartItems.Sum(i => i.Product.Price * i.Quantity),
                        OrderDetails = new List<OrderItems>()
                    };

                    foreach (var cartItem in cart.cartItems)
                    {
                        var result = await _productService.UpdateStockAsync(cartItem.ProductId, cartItem.Quantity);
                        if (!result.Isuccess)
                        {
                            return new ServiceResponse(false, result.Message);
                        }
                         order.OrderDetails.Add(new OrderItems
                        {
                            ProductId = cartItem.ProductId,
                            OrderQuantity = cartItem.Quantity,
                            OdersPrice = cartItem.Product.Price,
                        });
                    }

                    await _cartService.ClearCart(userId);


                    await _orderRepository.AddOrderAsync(order);
                    await _orderRepository.SaveChanges();


                    await transaction.CommitAsync ();
                    return new ServiceResponse(true, "Đặt hàng thành công");
                }
                catch (SqlException ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse(false, "Có lỗi xảy ra 1 : " + ex.Message);
                }
            }
        }

        public async Task<ServiceResponse> ConfirmOrderAsync(int id)
        {
            var order = await _orderRepository.GetOrdersByIdAsync(id);
            if (order == null)
                return new ServiceResponse(false, "Đơn hàng không tồn tại");

            if (order.OrderStatus != Order.Status.Pending.ToString())
                return new ServiceResponse(false, "Đơn hàng đã được xác nhận");

            order.OrderStatus = Order.Status.Shipped.ToString();
            await _orderRepository.SaveChanges();

            return new ServiceResponse(true, "Xác nhận đơn hàng thành công");

        }
    }
}
