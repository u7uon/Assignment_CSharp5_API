using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_Backend.Models
{
    public class Order
    {
        public int? OrderId { get; set; }

        [ForeignKey(nameof(UserId))]
        public string UserId { get; set; }
        public User? User { get; set; }

        public DateTime? OrderDate { get; set; }

        public string PayMethod { get; set; }
        public  string OrderStatus { get; set; } 

        public string Address { get; set; }


        public ICollection<OrderItems>? OrderDetails { get; set; }

         
        
        public enum Status
        {
            Pending,Confirmed , Shipped , Delivered, Canceled
        }

        

    }

  
}
