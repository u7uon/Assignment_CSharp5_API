using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Assignment_Backend.Models
{
    public class Cart
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartID { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<CartItem> cartItems { get; set; }

    }
}
