using Assignment_Backend.DTOs;
using Assignment_Backend.Models;
using Microsoft.EntityFrameworkCore.Query;

namespace Assignment_Backend.Interfaces
{
    public interface ICartRepository
    {
        Task<List<CartItem>> GetCartItemsByCartIdAsync(int cartId);
        Task<Cart> GetCartByUserIdAsync(string userId);
        Task<CartItem> GetCartItemByIdAsync(int cartItem);
        Task<bool> AddCartAsync(Cart cart);
        Task<bool> AddCartItemAsync(CartItem cartItem);
        Task<bool> UpdateCartItemAsync(CartItem cartItem);
        Task<bool> DeleteCartItemAsync(CartItem cartItem);
        Task<bool> DeleteCartAsync(Cart cart);
        Task<CartItem> GetCartItemByProductAsync(int cartId, int productId);

         Task SaveChangesAsync();




    }
}
