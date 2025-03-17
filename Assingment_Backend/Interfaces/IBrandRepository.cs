using Assignment_Backend.DTOs;
using Assignment_Backend.Models;

namespace Assignment_Backend.Interfaces
{
    public interface IBrandRepository
    {
        public Task<Brand> GetBrandAsync(int id);

        public Task<IEnumerable<Brand>> GetBrandsAsync();

        public Task<ServiceResponse> AddBrandAsync(Brand Brand);

        public Task<ServiceResponse> DeleteBrandAsync(int id);
        public Task<ServiceResponse> UpdateBrandAsync(int id, Brand Brand);
    }
}
