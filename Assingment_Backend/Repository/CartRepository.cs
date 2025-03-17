using Assignment_Backend.Data;
using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Assignment_Backend.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationContext _context;

        public CartRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<CartItem>> GetCartItemsByCartIdAsync(int cartId)
        {
            return await _context.CartItem
                .Where(x => x.CartId == cartId)
                .ToListAsync();
        }

        public async Task<Cart> GetCartByUserIdAsync(string userId)
        {
            return await _context.Cart.Include(x => x.cartItems).ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<CartItem> GetCartItemByIdAsync(int cartItemId)
        {
            return await _context.CartItem
                .FirstOrDefaultAsync(x => x.Id == cartItemId);
        }

        public async Task<bool> AddCartAsync(Cart cart)
        {
            try
            {
                await _context.Cart.AddAsync(cart);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> AddCartItemAsync(CartItem cartItem)
        {
            try
            {
                await _context.CartItem.AddAsync(cartItem);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateCartItemAsync(CartItem cartItem)
        {
            try
            {
                _context.CartItem.Update(cartItem);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCartItemAsync(CartItem cartItem)
        {
            try
            {
                _context.CartItem.Remove(cartItem);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCartAsync(Cart cart)
        {
            try
            {
                _context.Cart.Remove(cart);
                return await _context.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<CartItem> GetCartItemByProductAsync(int cartId, int productId)
        {
            return await _context.CartItem
                .FirstOrDefaultAsync(x => x.CartId == cartId && x.ProductId == productId);
        }

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}