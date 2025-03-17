using Assignment_Backend.DTOs;
using Assignment_Backend.Models;

namespace Assignment_Backend.Interfaces
{
    public interface ICategoryRepository
    {
        public  Task<Category> GetCategoryAsync(int id);

        public Task< (int ,IEnumerable<Category> )> GetCategoriesAsync(int currentPage, int pageSize);

        public Task<ServiceResponse> AddCategoryAsync(Category category);

        public Task<ServiceResponse> DeleteCategoryAsync(int id);
        public Task<ServiceResponse> UpdateCategoryAsync(int id , Category category);

       
    }
}
