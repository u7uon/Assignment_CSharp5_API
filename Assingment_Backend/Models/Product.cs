using Assingment_Backend.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Assignment_Backend.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        public string? Image {  get; set; }

        public int BrandId { get; set; }

        public bool IsActive { get; set; }


        public virtual Brand? Brand { get; set; }

        public int CategoryId { get; set; }


        public virtual Category? Category { get; set; }


        //public  ICollection<ProductSize> Sizes { get; set; }
    }
}
