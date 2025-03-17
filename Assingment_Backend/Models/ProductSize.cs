using Assignment_Backend.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assingment_Backend.Models
{
    [NotMapped]
    public class ProductSize
    {
        public int Id { get; set; } 
        // Khóa ngoại
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }

        public string Size { get; set; } // S, M, L
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}
