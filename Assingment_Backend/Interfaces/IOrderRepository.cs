using Assignment_Backend.Models;

namespace Assignment_Backend.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);

        Task<Order> GetOrdersByIdAsync(int id);

        Task AddOrderAsync(Order order);


        Task SaveChanges();

    }
}
