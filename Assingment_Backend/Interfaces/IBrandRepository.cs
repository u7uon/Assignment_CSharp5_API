using Assignment_Backend.DTOs;
using Assignment_Backend.Models;
using Assingment_Backend.Interfaces;

namespace Assignment_Backend.Interfaces
{
    public interface IBrandRepository : IGenericRepository<Brand>
    {
       

        public Task<Brand> GetBrandAsync(int id);

        public Task<(int, IEnumerable<Brand>)> GetAllBrandsAsync(int currentPage);

        //public Task<IEnumerable<Brand>> GetBrandsAsync();

        //public Task<ServiceResponse> AddBrandAsync(Brand Brand);

        //public Task<ServiceResponse> DeleteBrandAsync(int id);
        //public Task<ServiceResponse> UpdateBrandAsync(int id, Brand Brand);
    }
}
