using Assignment_Backend.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Assingment_Backend.DTOs;

namespace Assingment_Backend.Models
{
    public class CartItemsViewDTO
    {
        public int Id { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public ProductViewDTO Product { get; set; }
        public int Quantity { get; set; }

    }
}
