using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;

namespace Assignment_Backend.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _BrandRepository;

        public BrandService(IBrandRepository BrandRepository)
        {
            _BrandRepository = BrandRepository;
        }

        public async Task<ServiceResponse> AddBrandAsync(Brand Brand)
        {
            return await _BrandRepository.AddBrandAsync(Brand);
        }

        public async Task<ServiceResponse> DeleteBrandAsync(int id)
        {
            return await _BrandRepository.DeleteBrandAsync(id);
        }

        public async Task<ServiceResponse> UpdateBrandAsync(int id, Brand Brand)
        {
            return await _BrandRepository.UpdateBrandAsync(id, Brand);
        }

        public async Task<IEnumerable<Brand>> GetBrandAsync()
        {
            return await _BrandRepository.GetBrandsAsync();
        }

        public async Task<Brand> GetBrandAsync(int id)
        {
            return await _BrandRepository.GetBrandAsync(id);
        }
    }
}
