using Assignment_Backend.DTOs;
using Assignment_Backend.Models;
using Assingment_Backend.DTOs;

namespace Assignment_Backend.Interfaces
{
    public interface ICategoryService
    {
        public Task<ItemViewDTO<Category>> GetCategoriesAsync(int currentPage); 
        
        public Task<Category> GetCategoryAsync(int id);

        public Task<ServiceResponse> AddCategoryAsync(Category category); 

        public Task<ServiceResponse> UpdateCategoryAsync(int id ,Category category);

        public Task<ServiceResponse> DeleteCategoryAsync(int id);

    }
}
