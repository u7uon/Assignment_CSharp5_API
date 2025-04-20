namespace Assingment_Backend.DTOs
{
    public class TopItemDTO<T> where T  : class
    {
        public T Data { get; set; }

        public int Total { get; set; }
    }
}
