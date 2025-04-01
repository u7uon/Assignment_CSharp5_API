using Assignment_Backend.DTOs;
using Assignment_Backend.Models;
using Assingment_Backend.DTOs;

namespace Assignment_Backend.Interfaces
{
    public interface IProductService
    {
        public Task<SearchViewDTO> SearchByName(string keyword, int currentPage, FilterModel filter = null);
        public Task<Product> GetProductByIdAsync(int id);

        public Task<ItemViewDTO<ProductGetDto>> GetAllProductAsync(int currentPage);

        public Task<ServiceResponse> AddProductAsync(ProductDTO productDTO);

        public Task<ServiceResponse> DeleteProductAsync(int id);
        public Task<ServiceResponse> UpdateProductAsync(int id, ProductDTO productDto);

        public Task<ServiceResponse> UpdateStockAsync(int productId , int quantity);
    }
}
