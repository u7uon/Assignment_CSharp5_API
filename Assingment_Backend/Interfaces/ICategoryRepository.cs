using Assignment_Backend.DTOs;
using Assignment_Backend.Models;
using Assingment_Backend.Interfaces;

namespace Assignment_Backend.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        public  Task<Category> GetCategoryAsync(int id);

        public Task< (int ,IEnumerable<Category> )> GetCategoriesAsync(int currentPage);



        //public Task<ServiceResponse> AddCategoryAsync(Category category);

        //public Task<ServiceResponse> DeleteCategoryAsync(int id);
        //public Task<ServiceResponse> UpdateCategoryAsync(int id , Category category);

       
    }
}
