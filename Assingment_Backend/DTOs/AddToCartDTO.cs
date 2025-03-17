using Assignment_Backend.Models;

namespace Assignment_Backend.DTOs
{
    public class AddToCartDTO
    {
        public int ProductId { get; set; }
        public int quantity {  get; set; }
    }
}
