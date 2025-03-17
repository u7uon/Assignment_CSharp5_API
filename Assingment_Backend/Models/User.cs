using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Assignment_Backend.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public ICollection<Order>? Orders { get; set; } 
    }
}
