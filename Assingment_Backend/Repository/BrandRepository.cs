using Assignment_Backend.Data;
using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Assingment_Backend.Repository;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;

namespace Assignment_Backend.Repository
{
    public class BrandRepository : GenericRepository<Brand>, IBrandRepository
    {

        public BrandRepository(ApplicationContext context) : base (context)
        {
        }

        public async Task<(int, IEnumerable<Brand>)> GetAllBrandsAsync(int currentPage)
        {

            var total = await _dbSet.CountAsync();
            var brands = await _dbSet.Order()
                .Skip((currentPage - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .Select(x => new Brand
                {
                    BrandId = x.BrandId,
                    Name = x.Name,
                    Description = x.Description
                })
                .ToListAsync();

            return (total, brands);
        }

        //public async Task<ServiceResponse> AddBrandAsync(Brand Brand)
        //{
        //    if (Brand == null)
        //        return new ServiceResponse(false, "Thiếu dữ liệu đầu vào");

        //    using (var transaction = await context.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            await context.Brand.AddAsync(Brand);
        //            await context.SaveChangesAsync();
        //            await transaction.CommitAsync();
        //            return new ServiceResponse(true, "Thêm danh mục thành công");
        //        }
        //        catch (Exception ex)
        //        {
        //            await transaction.RollbackAsync();
        //            return new ServiceResponse(false, "Lỗi : " + ex.Message);
        //        }

        //    }
        //}

        //public async Task<ServiceResponse> DeleteBrandAsync(int id)
        //{
        //    if (id == 0)
        //        return new ServiceResponse(false, "Id = 0 ??");

        //    using (var transaction = await context.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            var Cate = await context.Brand.FindAsync(id);
        //            if (Cate == null)
        //                return new ServiceResponse(false, "Danh mục không tồn tại");

        //            context.Brand.Remove(Cate);
        //            await context.SaveChangesAsync();
        //            await transaction.CommitAsync();
        //            return new ServiceResponse(true, "Xóa danh mục thành công");
        //        }
        //        catch (Exception ex)
        //        {
        //            await transaction.RollbackAsync();
        //            return new ServiceResponse(false, "Lỗi : " + ex.Message);
        //        }

        //    }
        //}

        //public async Task<IEnumerable<Brand>> GetBrandsAsync()
        //{
        //    return await context.Brand.ToListAsync();
        //}

        public async Task<Brand> GetBrandAsync(int id)
        {
            return await _dbSet.Include(x => x.Products).FirstOrDefaultAsync(x => x.BrandId == id);

        }

        //public async Task<ServiceResponse> UpdateBrandAsync(int id, Brand Brand)
        //{
        //    if (Brand == null)
        //        return new ServiceResponse(false, "dữ liệu không hợp lệ");
        //    using (var transaction = await context.Database.BeginTransactionAsync())
        //    {
        //        try
        //        {
        //            var brand = await context.Brand.FindAsync(id);
        //            if (brand == null)
        //                return new ServiceResponse(false, "Danh mục không tồn tại");


        //            brand.Name = Brand.Name;
        //            brand.Description = Brand.Description; 

        //            context.Brand.Update(brand);
        //            await context.SaveChangesAsync();
        //            await transaction.CommitAsync();
        //            return new ServiceResponse(true, "Sửa danh mục thành công");
        //        }
        //        catch
        //        {
        //            await transaction.RollbackAsync();
        //            return new ServiceResponse(false, "Sửa thất bại ,  vui lòng thử lại");
        //        }
        //    }
        //}
    }
}
