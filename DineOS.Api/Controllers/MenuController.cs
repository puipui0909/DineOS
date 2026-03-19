using DineOS.Application.Common.Interfaces;
using DineOS.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace DineOS.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var menu = await _menuService.GetAllAsync();
            return Ok(menu);
        }

        [HttpGet("GetbyID/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _menuService.GetByIdAsync(id);

            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpGet("GetCategory/{categoryId}")]
        public async Task<IActionResult> GetByCategory(Guid categoryId)
        {
            var items = await _menuService.GetByCategoryAsync(categoryId);
            return Ok(items);
        }

        [HttpPost("CreateMenuItem")]
        public async Task<IActionResult> Create([FromBody] CreateMenuItemRequest request)
        {
            var id = await _menuService.CreateAsync(
                request.Name,
                request.Price,
                request.CategoryId
            );

            return CreatedAtAction(
                nameof(GetById),
                new { id },
                new { id }
            );
        }

        [HttpPut("UpdateItem/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateMenuItemRequest request)
        {
            await _menuService.UpdateAsync(id, request.Name, request.Price);

            return Ok();
        }

        [HttpDelete("DeletaItem/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _menuService.DeleteAsync(id);
            return Ok();
        }
    }
}
