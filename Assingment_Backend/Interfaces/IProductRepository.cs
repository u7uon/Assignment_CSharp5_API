using Assignment_Backend.DTOs;
using Assingment_Backend.DTOs; 
using Assignment_Backend.Models;
using System.Drawing;
using Assingment_Backend.Models;

namespace Assignment_Backend.Interfaces
{
    public interface IProductRepository
    {
        public Task<IEnumerable<Product>> GetProductsByNameAsync(string keyword);

        public Task<Product> GetProductByIdAsync(int id);
        
        public Task<(int ,IEnumerable<Product>)> GetAllProductAsync(int currentPage ,int pageSize);

        public Task<ReposResponse> AddProductAsync(Product product);

        public Task<ReposResponse> DeleteProductAsync(int id);
        public Task<ReposResponse> UpdateProductAsync(int id ,Product product);



        public Task<ReposResponse> AddProductSize(int productId ,  ProductSizeDTO[] sizes);

        public Task<ReposResponse> UpdateProductSize(int productId, ProductSizeDTO newsize);

        public Task<ReposResponse> DeleteProductSize(int productId, List<ProductSize> size);

        public Task SaveChangesAsync(); 

    }
}
