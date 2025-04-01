using Assignment_Backend.DTOs;
using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Assingment_Backend.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllProductAsync([FromQuery] int currentPage = 1)
        {
            var products = await _productService.GetAllProductAsync(currentPage);

            return Ok(products);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromForm] ProductDTO product)
        {
            var result = await _productService.AddProductAsync(product);

            return result.Isuccess ? Ok(result) : BadRequest(result.Message);
        }



        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, ProductDTO product)
        {
            var result = await _productService.UpdateProductAsync(id, product);

            return result.Isuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProductAsync(id);

            return result.Isuccess ? Ok(result) : BadRequest(result.Message);
        }

        [HttpGet]
        [Route("Search")]
        public async Task<IActionResult> Search(string keyWord, [FromQuery] FilterModel? filter, int currentPage)
        {
            return Ok(await _productService.SearchByName(keyWord, currentPage, filter));
        }


        [HttpGet("Images/{imageUrl}")]
        public async Task<IActionResult> GetImageAsync(string imageUrl)
        {
            // Giả sử đường dẫn ảnh được lưu trong thư mục "wwwroot/images"
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "products", imageUrl);

            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound("Không tìm thấy ảnh");
            }

            // Đọc nội dung file dưới dạng byte[]
            var imageBytes = await System.IO.File.ReadAllBytesAsync(imagePath);

            // Xác định content type dựa vào phần mở rộng file
            string contentType = imageUrl.EndsWith(".png") ? "image/png" :
                                 imageUrl.EndsWith(".jpg") || imageUrl.EndsWith(".jpeg") ? "image/jpeg" :
                                 imageUrl.EndsWith(".gif") ? "image/gif" : "application/octet-stream";

            return File(imageBytes, contentType);
        }


    }
}
