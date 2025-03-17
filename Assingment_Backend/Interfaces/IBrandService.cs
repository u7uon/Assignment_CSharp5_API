using Assignment_Backend.DTOs;
using Assignment_Backend.Models;

namespace Assignment_Backend.Interfaces
{
    public interface IBrandService
    {
        public Task<IEnumerable<Brand>> GetBrandAsync();

        public Task<Brand> GetBrandAsync(int id);

        public Task<ServiceResponse> AddBrandAsync(Brand Brand);

        public Task<ServiceResponse> UpdateBrandAsync(int id, Brand Brand);

        public Task<ServiceResponse> DeleteBrandAsync(int id);
    }
}
