using DineOS.Application.DTOs;
using DineOS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.Common.Interfaces
{
    public interface IMenuService
    {
        Task<List<MenuItemDto>> GetAllAsync();
        Task<MenuItemDto?> GetByIdAsync(Guid id);
        Task<List<MenuItemDto>> GetByCategoryAsync(Guid categoryId);

        Task<Guid> CreateAsync(string name, decimal price, Guid categoryId);
        Task UpdateAsync(Guid id, string name, decimal price);
        Task DeleteAsync(Guid id);
    }
}
