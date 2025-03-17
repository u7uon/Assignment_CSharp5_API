using Assignment_Backend.DTOs;
using Assignment_Backend.Models;
using System.Threading.Tasks;

namespace Assignment_Backend.Interfaces
{
    public interface IOrderService
    {
        Task< ServiceResponse> PlaceOrderAsync(Order order);

        Task< ServiceResponse> CancelOrderAsync(int OrderId);

        Task<Order> GetOrderByIdAsync(int id);

        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId); 

        Task<ServiceResponse> ConfirmOrderAsync (int id);


    }
}
