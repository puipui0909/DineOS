using DineOS.Application.DTOs;
using DineOS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DineOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // 🔥 bắt buộc login
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // 📌 GET: api/category
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryService.GetAllAsync();
            return Ok(result);
        }

        // 📌 POST: api/category
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            var id = await _categoryService.CreateAsync(dto.Name);
            return Ok(new { id });
        }


        // 📌 DELETE: api/category/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }

        //for customers
        [AllowAnonymous]
        [HttpGet("customer/by-table/{tableId}")]
        public async Task<IActionResult> GetByTable(Guid tableId)
        {
            var result = await _categoryService.GetByTableAsync(tableId);
            return Ok(result);
        }
    }
}