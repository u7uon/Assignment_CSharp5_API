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
                    return NotFound();
                return Ok(await _BrandService.GetBrandAsync(id));
            }

            [HttpGet]
            public async Task<IActionResult> GetAllBrand()
            {
                var categories = await _BrandService.GetBrandAsync();
                return Ok(categories);
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
                var result = await _BrandService.UpdateBrandAsync(id, Brand);
                if (result.Isuccess)
                    return Ok(result.Message);

                return BadRequest(result.Message);

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

