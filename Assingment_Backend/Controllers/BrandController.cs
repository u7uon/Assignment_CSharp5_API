using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Assignment_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _BrandService;

        public BrandController(IBrandService BrandService)
        {
            _BrandService = BrandService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandAsync(int id)
        {
            if (id == 0)
                return NotFound(new { Message = "Dữ liệu không hợp lệ" });

            var brand = await _BrandService.GetBrandAsync(id);

            return brand != null ? Ok(brand) : NotFound(new { Message = "Branb không tồn tại" });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBrand(int currentPage)
        {
            if (currentPage < 1)
                return BadRequest(new { Message = "Dữ liệu không hợp lệ" });

            var brands = await _BrandService.GetBrandsAsync(currentPage);
            return Ok(brands);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBrand([FromBody] Brand Brand)
        {
            var result = await _BrandService.AddBrandAsync(Brand);
            if (result.Isuccess)
                return Ok(result.Message);

            return BadRequest(result.Message);
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBrandAsync(int id, [FromBody] Brand Brand)
        {
            Console.WriteLine("Sửa aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

            var result = await _BrandService.UpdateBrandAsync(id, Brand);
            if (result.Isuccess)
                return Ok( result.Message);

            return BadRequest(new { Message = result.Message } );

        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrandAsync([FromRoute] int id)
        {
            var result = await _BrandService.DeleteBrandAsync(id);

            return result.Isuccess ? Ok(result.Message) : BadRequest(result.Message);
        }
    }
}

