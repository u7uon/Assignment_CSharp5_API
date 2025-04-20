using Assignment_Backend.Data;
using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Assingment_Backend.DTOs;
using Assingment_Backend.Models;
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

        public async Task<CartViewDTO> GetCartByUserIdAsync(string userId)
        {
            return await _context.Cart
                .Where(x => x.UserId == userId)
                .Select(cart => new CartViewDTO
                {
                    CartID = cart.CartID,
                    cartItems = cart.cartItems.Select(item => new CartItemsViewDTO
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        CartId = item.CartId,
                        Product = new ProductViewDTO
                        {
                            Id = item.Product.Id,
                            Name = item.Product.Name,
                            Price = item.Product.Price,
                            ImageUrl = item.Product.Image
                        }
                    }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();
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