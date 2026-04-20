using DineOS.Application.Common.Interfaces;
using DineOS.Application.DTOs;
using DineOS.Domain.Entities;
using DineOS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

public class TableService : ITableService
{
    private readonly IApplicationDbContext _context;

    public TableService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TableDto>> GetAllAsync()
    {
        return await _context.Tables
            .OrderBy(t => t.TableNumber)
            .Select(t => new TableDto
            {
                Id = t.Id,
                Name = $"T-{t.TableNumber:D2}",
                Status = t.Status.ToString()
            })
            .ToListAsync();
    }

    public async Task<TableDto> CreateAsync(CreateTableDto dto)
    {
        var table = new Table(dto.TableNumber, dto.RestaurantId);

        _context.Tables.Add(table);
        await _context.SaveChangesAsync();

        return new TableDto
        {
            Id = table.Id,
            Name = $"T-{table.TableNumber:D2}",
            Status = table.Status.ToString()
        };
    }
    public async Task CreateMultipleAsync(CreateMultipleTableDto dto)
    {
        var existingMax = await _context.Tables
            .Where(t => t.RestaurantId == dto.RestaurantId)
            .MaxAsync(t => (int?)t.TableNumber) ?? 0;

        var tables = new List<Table>();

        for (int i = 1; i <= dto.Quantity; i++)
        {
            var newTable = new Table(existingMax + i, dto.RestaurantId);
            tables.Add(newTable);
        }

        _context.Tables.AddRange(tables);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        var table = await _context.Tables.FindAsync(id);

        if (table == null)
            throw new Exception("Table not found");

        _context.Tables.Remove(table);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateStatusAsync(Guid id, string status)
    {
        var table = await _context.Tables.FindAsync(id);
        if (table == null)
            throw new Exception("Table not found");

        if (!Enum.TryParse<TableStatus>(status, out var newStatus))
            throw new Exception("Invalid status");

        switch (newStatus)
        {
            case TableStatus.Available:
                table.MarkAsAvailable();
                break;

            case TableStatus.Occupied:
                if (table.Status != TableStatus.Available)
                    throw new Exception("Table is not available");
                table.MarkAsOccupied();
                break;

            case TableStatus.Reserved:
                table.MarkAsReserved();
                break;

            case TableStatus.OutOfService:
                table.MarkAsOutOfService();
                break;
        }

        await _context.SaveChangesAsync();
    }
}