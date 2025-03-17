using Assignment_Backend.Models;

namespace Assignment_Backend.DTOs
{
    public class OrderDTO
    {

        public string PayMethod { get; set; }
        public string Address { get; set; }

        public List<OrderItems> OrderDetails { get; set; }
    }
}
