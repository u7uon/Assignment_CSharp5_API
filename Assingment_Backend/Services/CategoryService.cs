using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Assignment_Backend.Repository;
using Assingment_Backend.DTOs;

namespace Assignment_Backend.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ServiceResponse> AddCategoryAsync(Category category)
        {
            return await _categoryRepository.AddCategoryAsync(category);
        }

        public async Task<ServiceResponse> DeleteCategoryAsync(int id)
        {
            return await _categoryRepository.DeleteCategoryAsync(id);
        }

        public async Task<ServiceResponse> UpdateCategoryAsync(int id, Category category)
        {
            return await _categoryRepository.UpdateCategoryAsync(id, category);
        }

        public async Task<ItemViewDTO<Category>> GetCategoriesAsync(int curentPage)
        {
            var categoriesView = new ItemViewDTO<Category>();

            var (total, categories) = await _categoryRepository.GetCategoriesAsync(curentPage, categoriesView.PageSize);

            categoriesView.Items = categories;
            categoriesView.MaxPage = (int)Math.Ceiling((double)total / categoriesView.PageSize);
            categoriesView.CurrentPage = curentPage;


            return categoriesView;
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            return await _categoryRepository.GetCategoryAsync(id);
        }
    }
}

