using Assignment_Backend.Interfaces;
using Assignment_Backend.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Assignment_Backend.Services;

namespace Assignment_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryAsync(int id)
        {
            if (id == 0)
                return NotFound();
            return Ok(await _categoryService.GetCategoryAsync(id));
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]    
        public async Task<IActionResult> GetAllCategories(int currentPage)
        {
            var categories = await _categoryService.GetCategoriesAsync(currentPage);
            return Ok(categories);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            var result = await _categoryService.AddCategoryAsync(category);
            if (result.Isuccess)
                return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message });
        }

        [HttpPut("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategoryAsync(int id, [FromBody] Category category)
        {
            var result = await _categoryService.UpdateCategoryAsync(id, category);
            if (result.Isuccess)
                return Ok(new { message = result.Message });

            return BadRequest(new { message = result.Message });

        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategoryAsync([FromRoute]int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);

            return result.Isuccess ? Ok(result.Message) : BadRequest(result.Message);
        }


        [HttpGet("top")]
        public async Task<IActionResult> GetTopCategory()
        {
            return Ok(await _categoryService.GetTopCategory());
        }

        [HttpGet("All")]
        public async Task<IActionResult> All()
        {
            return Ok(await _categoryService.All());
        }

    }

}
