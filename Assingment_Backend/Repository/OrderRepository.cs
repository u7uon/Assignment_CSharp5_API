using Assignment_Backend.Data;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment_Backend.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationContext _context;

        public OrderRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task AddOrderAsync(Order order)
        {
            await _context.Order.AddAsync(order);   
            await _context.SaveChangesAsync();
        }

        public async Task<Order> GetOrdersByIdAsync(int id)
        {
            return await _context.Order.FirstOrDefaultAsync(x => x.OrderId == id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _context.Order.Where(x => x.UserId == userId).Include(x => x.OrderDetails).ThenInclude(x => x.Product).OrderByDescending(x => x.OrderDate).ToListAsync() ;
        }

        public async Task SaveChanges()
        {
           await _context.SaveChangesAsync();
        }
    }
}
