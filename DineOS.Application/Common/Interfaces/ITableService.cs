using DineOS.Application.DTOs;

public interface ITableService
{
    Task<List<TableDto>> GetAllAsync();
    Task<TableDto> CreateAsync(CreateTableDto dto);
    Task UpdateStatusAsync(Guid id, string status);
    Task CreateMultipleAsync(CreateMultipleTableDto dto);
    Task DeleteAsync(Guid id);
}