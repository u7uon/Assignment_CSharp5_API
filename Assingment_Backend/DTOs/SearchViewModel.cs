using Assignment_Backend.DTOs;
using Assignment_Backend.Models;

namespace Assignment_Backend.DTOs
{
    public class SearchViewModel
    {
        public string KeyWord { get; set; }
        public ICollection<Product> Products { get; set; }
        public int CurrentPage { get; set; }

        public int MaxSize { get; set; }

        public ICollection<Brand> brands { get; set; }
        public List<Category> categories { get; set; }

        public FilterModel? filterr { get; set; }

    }
}
