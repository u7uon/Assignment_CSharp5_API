using Assignment_Backend.Data;
using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Assignment_Backend.Repository;
using Assingment_Backend.DTOs;
using Assingment_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Text.Json.Serialization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace Assignment_Backend.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ApplicationContext _context;
        private const int PAGE_SIZE = 10; 

        public ProductService(IProductRepository productRepository, ApplicationContext context)
        {
            _productRepository = productRepository;
            _context = context;
        }

        public async Task<ServiceResponse> AddProductAsync(ProductDTO productDTO)
        {
            if (productDTO == null)
                return new ServiceResponse(false, "Dữ liệu không hợp lệ");


            //var ProductSizes = ConvertSize(productDTO.Sizes);

            try
            {
                var imageUrl = await UploadImageFile(productDTO.ImageFile);


                var product = new Product
                {
                    Name = productDTO.Name,
                    Description = productDTO.Description,
                    Quantity = productDTO.Quantity,
                    Price = productDTO.Price,
                    Image = imageUrl,
                    BrandId = productDTO.BrandId,
                    CategoryId = productDTO.CategoryId
                };



                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangesAsync();



                return new ServiceResponse(true, "Thêm sản phẩm thành công");
            }
            catch (SqlException ex)
            {
                return new ServiceResponse(false, "Xảy ra lỗi khi thêm sản phẩm" + ex.Message);
            }



        }

        public async Task<ServiceResponse> DeleteProductAsync(int id)
        {
            try
            {
                // Kiểm tra sản phẩm tồn tại
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                    return new ServiceResponse(false, "Sản phẩm không tồn tại");

                // Xóa ảnh sản phẩm nếu có
                if (!string.IsNullOrEmpty(product.Image))
                {
                    await DeleteImage(product.Image);
                }


                // Xóa sản phẩm
                await _productRepository.DeleteAsync(product);
                await _productRepository.SaveChangesAsync();

                return new ServiceResponse(true, "Xóa sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, "Lỗi khi xóa sản phẩm: " + ex.Message);
            }
        }

        public async Task<ItemViewDTO<ProductGetDto>> GetAllProductAsync(int currentPage,bool status)
        {
            var ProductsView = new ItemViewDTO<ProductGetDto>();

            var (total, products) = await _productRepository.GetAllAsync(currentPage ,status);

            ProductsView.Items = products;
            ProductsView.MaxPage = (int)Math.Ceiling((double)total / ProductsView.PageSize);
            ProductsView.CurrentPage = currentPage;

            return ProductsView;
        }

        public Task<Product> GetProductByIdAsync(int id)
        {
            return _productRepository.GetProductByIdAsync(id);
        }

        public async Task<SearchViewDTO> SearchByName(string keyword, int currentPage,  FilterModel filter = null)
        {
            var productQuery =  _productRepository.SearchByName( keyword);

            var total = await productQuery.CountAsync();

            var categories = await productQuery.Select(
                    x => new Category
                    {
                        Id = x.CategoryId,
                        Name = x.Category.Name
                    }
                ).Distinct().ToListAsync();

            var brands = await productQuery.Select(
                    x => new Brand
                    {
                        BrandId = x.BrandId,
                        Name = x.Brand.Name
                    }
                ).Distinct().ToListAsync();

            var products = await Filter(filter, productQuery)
                .Skip((currentPage - 1) * PAGE_SIZE)
                .Take(PAGE_SIZE)
                .Select(x => new ProductViewDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    ImageUrl = x.Image
                })
                .ToListAsync();


            return new SearchViewDTO()
            {
                Item = products,
                KeyWord = keyword,
                MaxPage =  (int)Math.Ceiling((double)total / 10),
                Categories = categories,
                Brands = brands,
                FilterModel = filter
            };
        }


        public async Task<ServiceResponse> UpdateProductAsync(int id, ProductDTO productDto)
        {
            if (productDto == null)
                return new ServiceResponse(false, "Dữ liệu không hợp lệ");


            try
            {
                // Kiểm tra sản phẩm tồn tại
                var existingProduct = await _productRepository.GetProductByIdAsync(id);
                if (existingProduct == null)
                    return new ServiceResponse(false, "Sản phẩm không tồn tại");

                // Upload ảnh mới nếu có
                if (productDto.ImageFile != null)
                {
                    // Xóa ảnh cũ
                    if (!string.IsNullOrEmpty(existingProduct.Image))
                    {
                        await DeleteImage(existingProduct.Image);
                    }
                    // Upload ảnh mới
                    existingProduct.Image = await UploadImageFile(productDto.ImageFile);
                }

                // Cập nhật thông tin cơ bản
                existingProduct.Name = productDto.Name;
                existingProduct.Description = productDto.Description;
                existingProduct.BrandId = productDto.BrandId;
                existingProduct.CategoryId = productDto.CategoryId;
                existingProduct.Price = productDto.Price;
                existingProduct.Quantity = productDto.Quantity;




                // Lưu thay đổi
                await _productRepository.UpdateAsync(existingProduct);

                await _productRepository.SaveChangesAsync();

                return new ServiceResponse(true, "Cập nhật sản phẩm thành công");
            }
            catch (SqlException ex)
            {
                return new ServiceResponse(false, "Lỗi khi cập nhật sản phẩm: " + ex.Message);
            }
        }


        private IQueryable<Product> Filter(FilterModel filter, IQueryable<Product> products)
        {
            if (filter == null) return products;

            if (filter.Brands?.Any() == true)
                products = products.Where(x => filter.Brands.Contains(x.BrandId));

            if (filter.Categories?.Any() == true)
                products = products.Where(x => filter.Categories.Contains(x.CategoryId));

            if (filter.MaxPrice.HasValue)
                products = products.Where(x => x.Price <= filter.MaxPrice);

            if (filter.MinPrice.HasValue)
                products = products.Where(x => x.Price >= filter.MinPrice);

            products = filter.Ascending switch
            {
                1 => products.OrderByDescending(x => x.Price),
                2 => products.OrderBy(x => x.Price),
                _ => products
            };

            return products;
        }

        public async Task<ServiceResponse> UpdateStockAsync(int productId, int quantity)
        {
            var product = await GetProductByIdAsync(productId);

            if (product == null)
                return new ServiceResponse(false, "Sản phẩm không tồn tại");

            if(quantity < 1) 
                return new ServiceResponse(false, "Số lượng phải lớn hơn 0");

            if (quantity > product.Quantity)
                return new ServiceResponse(false, $"Sản  phẩm :{product.Name} vượt quá số lượng tồn kho");

            product.Quantity -= quantity;
            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            return new ServiceResponse(true, "Cập nhật thành công");
        }


        private async Task<string> UploadImageFile(IFormFile ImageFile)
        {
            try
            {
                if (ImageFile == null || ImageFile.Length == 0)
                    return null;

                // Kiểm tra định dạng file
                var extension = Path.GetExtension(ImageFile.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowedExtensions.Contains(extension))
                    throw new Exception("Định dạng file không hợp lệ. Chỉ chấp nhận jpg, jpeg, png, gif");

                // Kiểm tra kích thước file (vd: max 5MB)
                if (ImageFile.Length > 5 * 1024 * 1024)
                    throw new Exception("Kích thước file không được vượt quá 5MB");

                // Tạo tên file unique
                var fileName = $"{Guid.NewGuid()}{extension}";

                // Tạo đường dẫn lưu file
                var uploadPath = Path.Combine("wwwroot", "images", "products");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                var filePath = Path.Combine(uploadPath, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                // Trả về đường dẫn relative để lưu DB
                return Path.Combine(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi upload file: {ex.Message}");
            }
        }

        private async Task DeleteImage(string FileName)
        {
            if (string.IsNullOrEmpty(FileName))
                return;

            var OldImagePath = Path.Combine("wwwroot", "products", FileName);
            try
            {
                if (File.Exists(OldImagePath))
                    File.Delete(OldImagePath);
            }
            catch (Exception)
            {
                return;
            }
        }

        public Task<IEnumerable<ProductViewDTO>> GetLatest()
        {
            return _productRepository.GetLatest();
        }

        public async Task<ServiceResponse> UpdateStatus(int id, bool status)
        {
            var product = await _productRepository.GetProductByIdAsync(id);
            if (product is null)
                return new ServiceResponse(false, "Sản phẩm không tồn tại");

            product.IsActive = status;
            await _context.SaveChangesAsync();

            return new ServiceResponse(true, "Cập nhật trạng thái thành công");
        }


    }
}
