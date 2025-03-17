using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment_Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductService _productService;

        public OrderService(IOrderRepository orderRepository ,IProductService productService)
        {
            _orderRepository = orderRepository;
            _productService = productService;
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

        public async Task<ServiceResponse> PlaceOrderAsync(Order order)
        {
            if (order.OrderDetails == null || !order.OrderDetails.Any())
                return new ServiceResponse(false, "Đơn hàng chưa có sản phẩm");

            try
            {

                foreach (var item in order.OrderDetails)
                {
                    var result = await _productService.UpdateStockAsync(item.ProductId, item.OrderQuantity);

                    if (!result.Isuccess)
                        return new ServiceResponse(false, result.Message); break;
                }

                order.OrderDate = DateTime.Now;
                order.OrderStatus = Order.Status.Pending.ToString();

                await _orderRepository.AddOrderAsync(order);
                await _orderRepository.SaveChanges();

                return new ServiceResponse(true, "Đặt hàng thành công");
            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, "Có lỗi xảy ra : " + ex.Message);
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
 