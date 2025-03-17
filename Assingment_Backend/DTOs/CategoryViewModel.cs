
using Assignment_Backend.Models;

namespace Assignment_Backend.DTOs
{
    public class CategoryViewModel
    {
        public Category Category { get; set; }
        public int curentPage { get; set; }
        public int MaxPage { get; set; }

    }
}
