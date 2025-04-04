using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Assingment_Backend.DTOs;

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
            if (Brand == null || string.IsNullOrEmpty(Brand.Name) || string.IsNullOrEmpty(Brand.Description))
                return new ServiceResponse(false, "Dữ liệu không hợp lệ");

            try
            {
                await _BrandRepository.AddAsync(Brand);
                await _BrandRepository.SaveChangesAsync();

                return new ServiceResponse(true, "Thêm thương hiệu thành công");
            }
            catch
            {
                return new ServiceResponse(false, "Đã xảy ra lỗi khi thêm thương hiệu ");
            }


        }

        public async Task<ServiceResponse> DeleteBrandAsync(int id)
        {
            if (id == 0)
                return new ServiceResponse(false, "Dữ liệu không hợp lệ");

            var brand = await _BrandRepository.GetBrandAsync(id);

            if (brand == null)
                return new ServiceResponse(false, "Brand không tồn tại ID này");

            try
            {
                await _BrandRepository.DeleteAsync(brand);
                await _BrandRepository.SaveChangesAsync();
                return new ServiceResponse(true, "Xóa brand thành công");
            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, "Đã xảy ra lỗi khi xóa brand : " + ex.Message);
            }
        }

        public async Task<ServiceResponse> UpdateBrandAsync(int id, Brand BrandDTO)    
        {
            if (BrandDTO == null || string.IsNullOrEmpty(BrandDTO.Name) || string.IsNullOrEmpty(BrandDTO.Description))
                return new ServiceResponse(false, "Dữ liệu không hợp lệ");

            var brand = await _BrandRepository.GetBrandAsync(id);

            if (brand == null)
                return new ServiceResponse(false, "Brand không tồn tại ID này");

            brand.Name = BrandDTO.Name;
            brand.Description = BrandDTO.Description;

            try
            {
                await _BrandRepository.UpdateAsync(brand);
                await _BrandRepository.SaveChangesAsync();

                return new ServiceResponse(true, "Sửa thương hiệu thành công");
            }
            catch
            {
                return new ServiceResponse(false, "Đã xảy ra lỗi khi thêm thương hiệu ");
            }

        }

        public async Task<ItemViewDTO<Brand>> GetBrandsAsync(int currentPage)
        {
            if (currentPage == 0)
                return new ItemViewDTO<Brand>();

            var (total, brands) = await _BrandRepository.GetAllBrandsAsync(currentPage);
            var result = new ItemViewDTO<Brand>
            {
                Items = brands,
                CurrentPage = currentPage,
                MaxPage = (int)Math.Ceiling((double)total / 10)

            };
            return result;
        }

        public async Task<Brand> GetBrandAsync(int id)
        {
            return await _BrandRepository.GetBrandAsync(id);
        }
    }
}
