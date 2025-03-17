using Assignment_Backend.Data;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Microsoft.Data.SqlClient;
using Assignment_Backend.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Assingment_Backend.DTOs;

namespace Assignment_Backend.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationContext context;

        public CategoryRepository(ApplicationContext context)
        {
            this.context = context;
        }

        public async Task<ServiceResponse> AddCategoryAsync(Category category)
        {
            if (category == null)
                return new ServiceResponse(false, "Thiếu dữ liệu đầu vào");

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    await context.Categories.AddAsync(category);
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse(true, "Thêm danh mục thành công");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse(false, "Lỗi : " + ex.Message);
                }

            }
        }

        public async Task<ServiceResponse> DeleteCategoryAsync(int id)
        {
            if (id == 0)
                return new ServiceResponse(false, "Id = 0 ??");

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var Cate = await context.Categories.FindAsync(id);
                    if (Cate == null)
                        return new ServiceResponse(false, "Danh mục không tồn tại");

                    context.Categories.Remove(Cate);
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse(true, "Xóa danh mục thành công");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse(false, "Lỗi : " + ex.Message);
                }

            }
        }

        public async Task< (int , IEnumerable<Category> )> GetCategoriesAsync(int currentPage, int pageSize)
        {
            if (currentPage < 1)
                currentPage = 1;

            var total = await context.Category.CountAsync();

            var categories = await context.Category
                .OrderBy(x => x.Id).
                Skip((currentPage - 1) * pageSize).
                Take(10).
                AsNoTracking()
                .ToListAsync();
            return (total, categories );
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            return await context.Categories
                .Where(c => c.Id == id)
                .Select(c => new Category
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Image = c.Image,
                    Products = c.Products.Select(p => new Product
                    {
                        Id = p.Id ,
                        CategoryId =  p.CategoryId,
                        Name = p.Name,
                        Description = p.Description,
                        Image = p.Image,
                        Brand = new Brand
                        {
                            BrandId = p.BrandId,
                            Name = p.Brand.Name
                        }
                    }).ToList()
                }).FirstOrDefaultAsync();
        }


        public async Task<ServiceResponse> UpdateCategoryAsync(int id, Category category)
        {
            if (category == null)
                return new ServiceResponse(false, "dữ liệu không hợp lệ");
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var cate = await context.Categories.FindAsync(id);
                    if (cate == null)
                        return new ServiceResponse(false, "Danh mục không tồn tại");

                    cate.Name = category.Name;
                    cate.Description = category.Description; 

                    context.Categories.Update(cate);
                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ServiceResponse(true, "Sửa danh mục thành công");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    return new ServiceResponse(false, "Sửa thất bại ,  vui lòng thử lại");
                }



            }

        }
    }
}
