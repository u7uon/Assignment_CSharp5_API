using Assignment_Backend.Models;

namespace Assignment_Backend.Interfaces
{
    public interface IOrderRepository
    {
        Task<(int,IEnumerable<Order> ) > GetOrdersAsync(string userId ,string status, int currentPage);

        Task<Order> GetOrdersByIdAsync(int id);

        Task AddOrderAsync(Order order);

        Task SaveChanges();

    }
}
