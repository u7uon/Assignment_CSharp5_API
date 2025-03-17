namespace Assignment_Backend.DTOs
{
    public class FilterModel
    {

        // public List<int> Brands { get; set; } // 
        public int? Ascending { get; set; }
        public List<int>? Brands { get; set; }
        public List<int>? Categories { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

    }
}
