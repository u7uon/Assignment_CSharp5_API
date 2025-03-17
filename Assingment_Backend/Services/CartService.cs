using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;

namespace Assignment_Backend.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductService _productService;

        public CartService(ICartRepository cartRepository, IProductService productService)
        {
            _cartRepository = cartRepository;
            _productService = productService;
        }
        public async Task<ServiceResponse> AddToCart(string userId, int productId, int quantity)
        {
            if (userId == null)
                return new ServiceResponse(false, "User không hợp lệ");
            if (quantity <= 0)
                return new ServiceResponse(false, "Số lượng không hợp lệ");

            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
                return new ServiceResponse(false, "Sản phẩm không tồn tại");
            if (quantity > product.Quantity)
                return new ServiceResponse(false, "Số lượng vượt quá giới hạn");

            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                var newCart = new Cart { UserId = userId };
                var result = await _cartRepository.AddCartAsync(newCart);
                if (!result)
                {
                    return new ServiceResponse(false, "Không thể tạo giỏ hàng");
                }
                cart = newCart;
            }

            var exitingItem = await _cartRepository.GetCartItemByProductAsync(productId, cart.CartID);

            if (exitingItem != null)
            {
                exitingItem.Quantity += quantity;

                return await _cartRepository.UpdateCartItemAsync(exitingItem)
                ? new ServiceResponse(true, "Thêm vào giỏ hàng thành công")
                : new ServiceResponse(false, "Không thể thêm vào giỏ , thử lại sau");
            }


            exitingItem = new CartItem
            {
                CartId = cart.CartID,
                ProductId = productId,
                Quantity = quantity,
            };

            return await _cartRepository.AddCartItemAsync(exitingItem)
                ? new ServiceResponse(true, "Thêm vào giỏ hàng thành công")
                : new ServiceResponse(false, "Không thể thêm vào giỏ , thử lại sau");
        }

        public async Task<Cart> GetCart(string userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null)
            {
                cart = new Cart()
                {
                    UserId = userId,
                };
                await _cartRepository.AddCartAsync(cart);
            }
            return cart;
        }

        public async Task<ServiceResponse> RemoveCartItem(int cartItemId)
        {
            if (cartItemId == 0)
                return new ServiceResponse(false, "Item không hợp lệ");

            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null)
                return new ServiceResponse(false, "Item không tồn tại");

            var result = await _cartRepository.DeleteCartItemAsync(cartItem);
            if(!result)
                return new ServiceResponse(false, "Không thể xóa item vui lòng thử lại");

            return new ServiceResponse(true, "Xóa item thành công"); 
        }

        public async Task<ServiceResponse> InscreaseItemByIdAsync(int cartItemId)
        {

            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null)
                return new ServiceResponse(false, "Item không tồn tại");

            var product = await _productService.GetProductByIdAsync(cartItem.ProductId); 
            if (product == null)
                return new ServiceResponse(false, "Sản phẩm không còn tồn tại");

            if (product.Quantity == cartItem.Quantity)
                return new ServiceResponse(false, "Số lượng đã đạt tối đa");

            cartItem.Quantity += 1; 
            await _cartRepository.SaveChangesAsync();
            return new ServiceResponse(true, "Tăng số lượng thành công");
        }
        public async Task<ServiceResponse> DescreaseItemByIdAsync(int cartItemId)
        {
            var cartItem = await _cartRepository.GetCartItemByIdAsync(cartItemId);
            if (cartItem == null)
                return new ServiceResponse(false, "Item không tồn tại");

            var product = await _productService.GetProductByIdAsync(cartItem.ProductId);
            if (product == null)
                return new ServiceResponse(false, "Sản phẩm không còn tồn tại");

            if (cartItem.Quantity == 1)
                return await RemoveCartItem(cartItem.Id); 

            cartItem.Quantity -= 1;
            await _cartRepository.SaveChangesAsync();

            return new ServiceResponse(true, "Giảm số lượng thành công");
        }
    }
}
