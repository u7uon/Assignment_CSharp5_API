using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Assignment_Backend.Repository;
using Assingment_Backend.DTOs;
using Assingment_Backend.Models;
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

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
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
                    Image = imageUrl,
                    BrandId = productDTO.BrandId,
                    CategoryId = productDTO.CategoryId
                };



                await _productRepository.AddProductAsync(product);
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
                await _productRepository.DeleteProductAsync(id);
                await _productRepository.SaveChangesAsync();

                return new ServiceResponse(true, "Xóa sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return new ServiceResponse(false, "Lỗi khi xóa sản phẩm: " + ex.Message);
            }
        }

        public async Task<ItemViewDTO<Product>> GetAllProductAsync(int currentPage)
        {
            var ProductsView = new ItemViewDTO<Product>();

            var (total, products) = await _productRepository.GetAllProductAsync(currentPage, ProductsView.PageSize);

            ProductsView.Items = products;
            ProductsView.MaxPage = (int)Math.Ceiling((double)total / ProductsView.PageSize);
            ProductsView.CurrentPage = currentPage;


            return ProductsView;
        }

        public Task<Product> GetProductByIdAsync(int id)
        {
            return _productRepository.GetProductByIdAsync(id);
        }

        public async Task<SearchViewDTO> GetProductsByNameAsync(string keyword, int currentPage, int pageSize, FilterModel filter = null)
        {
            var products = await _productRepository.GetProductsByNameAsync(keyword);

            var categories = products.Select(x => x.Category).Distinct().ToList();
            var brands = products.Select(x => x.Brand).Distinct().ToList();

            var filteredProducts = Filter(products, filter);

            return new SearchViewDTO()
            {
                Item = filteredProducts.Skip((currentPage - 1) * pageSize).Take(pageSize),
                KeyWord = keyword,
                PageSize = pageSize,
                MaxPage = (int)Math.Ceiling((double)filteredProducts.Count() / pageSize),
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
                await _productRepository.UpdateProductAsync(existingProduct.Id, existingProduct);

                await _productRepository.SaveChangesAsync();

                return new ServiceResponse(true, "Cập nhật sản phẩm thành công");
            }
            catch (SqlException ex)
            {
                return new ServiceResponse(false, "Lỗi khi cập nhật sản phẩm: " + ex.Message);
            }
        }


        private IEnumerable<Product> Filter(IEnumerable<Product> products, FilterModel filter)
        {
            if (filter == null)
                return products;

            if (filter.Brands != null)
                products = products.Where(x => filter.Brands.Contains(x.BrandId));

            if (filter.Categories != null)
                products = products.Where(x => filter.Categories.Contains(x.CategoryId));

            //if (filter.MinPrice.HasValue && filter.MaxPrice.HasValue)
            //    products = products.Where(x => x.Price >= filter.MinPrice.Value && x.Price <= filter.MaxPrice.Value);

            //if (filter.Ascending.HasValue)
            //    products = filter.Ascending == 1 ? products.OrderBy(x => x.Price) : products.OrderByDescending(x => x.Price);

            return products;
        }

        public async Task<ServiceResponse> UpdateStockAsync(int productId, int quantity)
        {
            var product = await GetProductByIdAsync(productId);

            if (product == null)
                return new ServiceResponse(false, "Sản phẩm không tồn tại");

            //if(quantity > product.Quantity)
            //    return new ServiceResponse(false, "Số lượng vượt quá tồn kho");

            //product.Quantity -= quantity;
            // var result =  await UpdateProductAsync(productId ,  product);

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

        private List<ProductSizeDTO> ConvertSize(string sizes)
        {
            if (sizes == null)
                throw new Exception("Thiếu size");

            return System.Text.Json.JsonSerializer.Deserialize<List<ProductSizeDTO>>(sizes);
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

    }
}
