using Assignment_Backend.Data;
using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Assignment_Backend.Repository;
using Assingment_Backend.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Assignment_Backend.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ApplicationContext _context;

        public CategoryService(ICategoryRepository categoryRepository, ApplicationContext co)
        {
            _categoryRepository = categoryRepository;
            _context = co;
        }

        public async Task<ServiceResponse> AddCategoryAsync(Category category)
        {
            if (category == null || string.IsNullOrEmpty(category.Name) || string.IsNullOrEmpty(category.Description))
                return new ServiceResponse(false, "Dữ liệu không hợp lệ");

            try
            {
                await _categoryRepository.AddAsync(category);
                await _categoryRepository.SaveChangesAsync();

                return new ServiceResponse(true, "Thêm danh mục thành công"); 

            }
            catch(Exception ex)
            {
                return new ServiceResponse(false, "Gặp lỗi khi thêm danh mục " + ex.Message);
            }    
        }

        public async Task<ServiceResponse> DeleteCategoryAsync(int id)
        {
            if (id == 0 )
                return new ServiceResponse(false, "Dữ liệu không hợp lệ");

            var cate = await _categoryRepository.GetCategoryAsync(id);

            if (cate == null)
                return new ServiceResponse(false, "Danh mục không tồn tại");

            try
            {
                await _categoryRepository.DeleteAsync(cate);
                await _categoryRepository.SaveChangesAsync();

                return new ServiceResponse(true, "Xóa danh mục thành cônng ");
            }
            catch(Exception ex)
            {
                return new ServiceResponse(false, "Xảy ra lỗi khi xóa danh mục: " + ex.Message);
            }
        }

        public async Task<ServiceResponse> UpdateCategoryAsync(int id, Category category)
        {
            if (id == 0 || category == null || string.IsNullOrEmpty(category.Name) || string.IsNullOrEmpty(category.Description))
                return new ServiceResponse(false, "Dữ liệu không hợp lệ");

            var cate = await _categoryRepository.GetCategoryAsync(id);

            if (cate == null)
                return new ServiceResponse(false, "Danh mục không tồn tại");

            cate.Name = category.Name;
            cate.Description = category.Description;

            try
            {
                await _categoryRepository.UpdateAsync(cate);
                await _categoryRepository.SaveChangesAsync();

                return new ServiceResponse(true, "Sửa danh mục thành công");

            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, "Gặp lỗi khi sửa danh mục " + ex.Message);
            }
        }

        public async Task<ItemViewDTO<Category>> GetCategoriesAsync(int curentPage)
        {
            var categoriesView = new ItemViewDTO<Category>();

            var (total, categories) = await _categoryRepository.GetCategoriesAsync(curentPage);

            categoriesView.Items = categories;
            categoriesView.MaxPage = (int)Math.Ceiling((double)total / categoriesView.PageSize);
            categoriesView.CurrentPage = curentPage;


            return categoriesView;
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            return await _categoryRepository.GetCategoryAsync(id);
        }

        public async Task<IEnumerable<TopItemDTO<Category>>> GetTopCategory()
        {
            return await _categoryRepository.GetTopCategory();
        }


        public async Task<IEnumerable<CategoryDTO>> All()
        {
            return await _context.Category.Select(
                   x => new CategoryDTO
                   {
                       Id = x.Id,
                       Name = x.Name
                   }
                ).ToListAsync();
        }
    }
}

