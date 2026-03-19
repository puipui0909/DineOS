using DineOS.Application.Common.Interfaces;
using DineOS.Application.DTOs;
using DineOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IApplicationDbContext _context;

        public MenuService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItemDto>> GetAllAsync()
        {
            return await _context.MenuItems.Where(x => x.IsAvailable).Select(x => new MenuItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                IsAvailable = x.IsAvailable
            })
            .ToListAsync();
        }

        public async Task<MenuItemDto?> GetByIdAsync(Guid id)
        {
            return await _context.MenuItems
                .Where(x => x.Id == id)
                .Select(x => new MenuItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    IsAvailable = x.IsAvailable
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<MenuItemDto>> GetByCategoryAsync(Guid categoryId)
        {
            return await _context.MenuItems
                .Where(x => x.CategoryId == categoryId && x.IsAvailable)
                .Select(x => new MenuItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    IsAvailable = x.IsAvailable
                })
                .ToListAsync();
        }

        public async Task<Guid> CreateAsync(string name, decimal price, Guid categoryId)
        {
            var item = new MenuItem(name, price, categoryId);

            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();

            return item.Id;
        }

        public async Task UpdateAsync(Guid id, string name, decimal price)
        {
            var item = await _context.MenuItems.FindAsync(id);

            if (item == null)
                throw new KeyNotFoundException("Menu item not found");

            item.UpdateName(name);
            item.UpdatePrice(price);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var item = await _context.MenuItems.FindAsync(id);

            if (item == null)
                throw new KeyNotFoundException("Menu item not found");

            _context.MenuItems.Remove(item);

            await _context.SaveChangesAsync();
        }
    }
}
