using DineOS.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TableController : ControllerBase
{
    private readonly ITableService _tableService;

    public TableController(ITableService tableService)
    {
        _tableService = tableService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _tableService.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTableDto dto)
    {
        return Ok(await _tableService.CreateAsync(dto));
    }

    [HttpPost("bulk")]
    public async Task<IActionResult> CreateMultiple(CreateMultipleTableDto dto)
    {
        await _tableService.CreateMultipleAsync(dto);
        return Ok();
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
    {
        await _tableService.UpdateStatusAsync(id, status);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _tableService.DeleteAsync(id);
        return Ok();
    }
}