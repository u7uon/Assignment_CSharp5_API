using Assignment_Backend.Models;
using Assingment_Backend.DTOs;

namespace Assignment_Backend.DTOs
{
    public class SearchViewDTO
    {
        public string KeyWord { get; set; }
        public IEnumerable<ProductViewDTO>? Item { get; set; }

        public int CurrentPage; 

        public int PageSize { get; set; }

        public int MaxPage { get; set; }


        public IEnumerable<Brand> Brands { get; set; }

        public FilterModel FilterModel { get; set; }
        public List<Category> Categories { get; set; }
    }
}
