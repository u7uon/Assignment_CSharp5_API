namespace Assingment_Backend.DTOs
{
    public class ItemViewDTO<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; } = 10; 
        public int MaxPage { get; set; } 
    }
}
