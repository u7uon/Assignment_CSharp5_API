using Assignment_Backend.DTOs;
using Assignment_Backend.Models;
using Assingment_Backend.DTOs;

namespace Assignment_Backend.Interfaces
{
    public interface IBrandService
    {
        public Task<ItemViewDTO<Brand>> GetBrandsAsync(int currentPage);

        public Task<Brand> GetBrandAsync(int id);

        public Task<ServiceResponse> AddBrandAsync(Brand Brand);

        public Task<ServiceResponse> UpdateBrandAsync(int id, Brand Brand);

        public Task<ServiceResponse> DeleteBrandAsync(int id);

        public Task<IEnumerable<BrandDTO>> All();
    }
}
