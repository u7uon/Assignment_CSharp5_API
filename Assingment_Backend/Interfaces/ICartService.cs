using Assignment_Backend.DTOs;
using Assignment_Backend.Models;

namespace Assignment_Backend.Interfaces
{
    public interface ICartService
    {
        public Task<ServiceResponse> AddToCart(string  userId, int productId, int quantity);
        public Task<ServiceResponse> InscreaseItemByIdAsync(int cartItemId);
        public  Task<ServiceResponse> DescreaseItemByIdAsync(int cartItemId);

        public Task<ServiceResponse> RemoveCartItem(int cartItemId);
        public Task<Cart> GetCart(string userId);

    }
}
