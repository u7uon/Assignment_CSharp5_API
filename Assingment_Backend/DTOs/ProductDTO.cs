using Assignment_Backend.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations;

namespace Assingment_Backend.DTOs
{
    public class ProductDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
        public IFormFile? ImageFile { get; set; }

        public int BrandId { get; set; }
        public int CategoryId { get; set; }

        //public string Sizes { get; set; }
    }
}
