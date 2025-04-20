using Assignment_Backend.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Assingment_Backend.Models;

namespace Assingment_Backend.DTOs
{
    public class CartViewDTO
    {
        public int CartID { get; set; }

        public ICollection<CartItemsViewDTO> cartItems { get; set; }
    }
}
