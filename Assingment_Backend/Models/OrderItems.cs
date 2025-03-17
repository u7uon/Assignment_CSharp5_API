using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_Backend.Models
{
    public class OrderItems
    {
        public int Id { get; set; }


        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public decimal? OdersPrice { get; set; }

        public int OrderQuantity { get; set; }

        public decimal? TotalPrice {  get; set; }



    }
}
