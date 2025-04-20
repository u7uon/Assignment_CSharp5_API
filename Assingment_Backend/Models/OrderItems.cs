using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_Backend.Models
{
    public class OrderItems
    {
        public int Id { get; set; }

        [ForeignKey(nameof(OrderId))]        
        public int OrderId { get; set; }
        public Order? Order { get; set; }


        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        public decimal? OdersPrice { get; set; }

        public int OrderQuantity { get; set; }

        public decimal? TotalPrice {  get; set; }

    }
}
