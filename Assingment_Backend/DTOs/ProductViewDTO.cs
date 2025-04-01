namespace Assingment_Backend.DTOs
{
    public class ProductViewDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        public int BrandId { get; set; }
        public int CategoryId { get; set; }
    }
}
