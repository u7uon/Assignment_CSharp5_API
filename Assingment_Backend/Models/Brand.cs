using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Assignment_Backend.Models
{
    public class Brand
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BrandId { get; set; }

        [Required]
        [MaxLength(100 , ErrorMessage = "Tối đa 100 ký tự")]
        public string Name { get; set; }


        public string? Description { get; set; }

        public ICollection<Product >? Products { get; set; }
    }
}
