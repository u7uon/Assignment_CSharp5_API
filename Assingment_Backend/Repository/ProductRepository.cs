using Assignment_Backend.Data;
using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Assingment_Backend.DTOs;
using Assingment_Backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace Assignment_Backend.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationContext _context;

        public ProductRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<ReposResponse> AddProductAsync(Product product)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Product.Add(product);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ReposResponse(true, "Thêm sản phẩm thành công");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ReposResponse(false, ex.Message);
                }
            }
        }

        public async Task<ReposResponse> DeleteProductAsync(int id)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var product = await _context.Product.FirstAsync(x => x.Id == id);
                    if (product == null)
                        return new ReposResponse(false, "Sản phẩm không tồn tại");

                    _context.Product.Remove(product);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ReposResponse(true, "Thêm sản phẩm thành công");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ReposResponse(false, ex.Message);
                }
            }
        }

        public async Task<( int , IEnumerable<Product> )> GetAllProductAsync(int currentPage ,int pageSize)
        {
            if (currentPage < 1)
                currentPage = 1;

            var total = await _context.Product.CountAsync(); 

            var products = await _context.Product
                .Include(x => x.Brand)
                .Include(x => x.Category)
                .OrderBy(x => x.Id).
                Skip( (currentPage -1 ) * pageSize ).
                Take(10).
                AsNoTracking() 
                .ToListAsync();
            return (total,products);
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await _context.Product
          .AsNoTracking()  // Thêm khi chỉ cần đọc data
          .FirstOrDefaultAsync(x => x.Id == id);


            return product;
        }

        public async Task<IEnumerable<Product>> GetProductsByNameAsync(string keyword)
        {
            return await _context.Product
       .Include(x => x.Brand)
       .Include(x => x.Category)
       .Where(x => x.Name.Contains(keyword))
       .AsNoTracking()
       .ToListAsync();
        }

        public async Task<ReposResponse> UpdateProductAsync(int id , Product uproduct)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {

                     _context.Product.Update(uproduct);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ReposResponse(true, "Cập nhật sản phẩm thành công "); 

                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();

                    return new ReposResponse(false, "Cập nhật sản phẩm thất bại \n Lỗi :" + ex.Message); 
                }
            }
        }


        public async Task<ReposResponse> AddProductSize(int productId, ProductSizeDTO[] sizes)
        {
            try
            {
                // Kiểm tra sản phẩm tồn tại
                var product = await _context.Product.FindAsync(productId);
                if (product == null)
                    return new ReposResponse(false, "Sản phẩm không tồn tại");

                // Kiểm tra size trùng
                var existingSizes = await _context.ProductSizes
                    .Where(x => x.ProductId == productId)
                    .Select(x => x.Size)
                    .ToListAsync();

                foreach (var size in sizes)
                {
                    if (existingSizes.Contains(size.Size))
                        return new ReposResponse(false, $"Size {size.Size} đã tồn tại");

                    await _context.ProductSizes.AddAsync(new ProductSize()
                    {
                        ProductId = productId,
                        Size = size.Size,
                        Price = size.Price,
                        StockQuantity = size.StockQuantity
                    });
                }

                await _context.SaveChangesAsync();
                return new ReposResponse(true, "Thêm size thành công");
            }
            catch (Exception ex)
            {
                return new ReposResponse(false, "Lỗi khi thêm size: " + ex.Message);
            }
        }

        public async Task<ReposResponse> UpdateProductSize(int productId, ProductSizeDTO newsize)
        {
            try
            {
                var size = await _context.ProductSizes
                    .FirstOrDefaultAsync(x => x.ProductId == productId && x.Id == newsize.Id);

                if (size == null)
                    return new ReposResponse(false, "Size không tồn tại");

                // Kiểm tra nếu đổi tên size thì không được trùng
                if (size.Size != newsize.Size)
                {
                    var existingSize = await _context.ProductSizes
                        .AnyAsync(x => x.ProductId == productId && x.Size == newsize.Size);

                    if (existingSize)
                        return new ReposResponse(false, $"Size {newsize.Size} đã tồn tại");
                }

                size.Size = newsize.Size;
                size.Price = newsize.Price;
                size.StockQuantity = newsize.StockQuantity;

                await _context.SaveChangesAsync();
                return new ReposResponse(true, "Cập nhật size thành công");
            }
            catch (Exception ex)
            {
                return new ReposResponse(false, "Lỗi khi cập nhật size: " + ex.Message);
            }
        }

        public async Task<ReposResponse> DeleteProductSize(int productId, List<ProductSize> size)
        {
            try
            {
                var productSize = await _context.ProductSizes
                    .FirstOrDefaultAsync(x => x.ProductId == productId );

                if (productSize == null)
                    return new ReposResponse(false, "Size không tồn tại");

                // Kiểm tra có đơn hàng nào đang dùng size này không
                //var hasOrders = await _context.OrderDetail
                //    .AnyAsync(x => x.ProductId == productId && x.Size == size.Size);

                //if (hasOrders)
                //    return new ReposResponse(false, "Không thể xóa size đã có đơn hàng");

                _context.ProductSizes.RemoveRange(productSize);
                await _context.SaveChangesAsync();
                return new ReposResponse(true, "Xóa size thành công");
            }
            catch (Exception ex)
            {
                return new ReposResponse(false, "Lỗi khi xóa size: " + ex.Message);
            }
        }


        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}
