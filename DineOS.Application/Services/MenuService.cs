using DineOS.Application.Common.Interfaces;
using DineOS.Application.DTOs;
using DineOS.Domain.Entities;
using Microsoft.AspNetCore.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MenuService(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        private Guid GetRestaurantId()
        {
            var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("restaurantId");

            if (claim == null)
                throw new UnauthorizedAccessException("Không tìm thấy restaurantId trong token");

            return Guid.Parse(claim.Value);
        }

        public async Task<List<MenuItemDto>> GetAllAsync()
        {
            var restaurantId = GetRestaurantId();
            return await _context.MenuItems.Select(x => new MenuItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Price = x.Price,
                IsAvailable = x.IsAvailable,
                ImageUrl = x.ImageUrl,
                CategoryId = x.CategoryId
            })
            .ToListAsync();
        }

        public async Task<MenuItemDto?> GetByIdAsync(Guid id)
        {
            var restaurantId = GetRestaurantId();
            return await _context.MenuItems
                .Where(x => x.Id == id)
                .Select(x => new MenuItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    IsAvailable = x.IsAvailable,
                    CategoryId = x.CategoryId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<MenuItemDto>> GetByCategoryAsync(Guid categoryId)
        {
            var restaurantId = GetRestaurantId();
            return await _context.MenuItems
                .Where(x => x.CategoryId == categoryId
                     && x.IsAvailable)
                .Select(x => new MenuItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    IsAvailable = x.IsAvailable,
                    CategoryId = x.CategoryId
                })
                .ToListAsync();
        }

        public async Task<Guid> CreateAsync(string name, decimal price, Guid categoryId, string imageUrl)
        {
            // 🔥 Lấy category
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
                throw new Exception("Category không tồn tại");

            // 🔥 Tạo item với RestaurantId đúng
            var item = new MenuItem(
                name,
                price,
                categoryId,
                imageUrl
            );

            _context.MenuItems.Add(item);
            await _context.SaveChangesAsync();

            return item.Id;
        }

        public async Task UpdateAsync(Guid id, string name, decimal price, string? imageUrl)
        {
            var restaurantId = GetRestaurantId();

            var item = await _context.MenuItems
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
                throw new KeyNotFoundException("Menu item not found");

            item.UpdateName(name);
            item.UpdatePrice(price);
            if (imageUrl != null)
            {
                item.UpdateImage(imageUrl);
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var restaurantId = GetRestaurantId();

            var item = await _context.MenuItems
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
                throw new KeyNotFoundException("Menu item not found");

            _context.MenuItems.Remove(item);

            await _context.SaveChangesAsync();
        }
        public async Task ToggleStatusAsync(Guid id)
        {
            var item = await _context.MenuItems
                .FirstOrDefaultAsync(x => x.Id == id);

            if (item == null)
                throw new KeyNotFoundException("Menu item not found");

            item.ToggleAvailability();

            await _context.SaveChangesAsync();
        }

        //for customers
        public async Task<List<MenuItemDto>> GetMenuByTableAsync(Guid tableId)
        {
            var table = await _context.Tables
                .FirstOrDefaultAsync(t => t.Id == tableId);

            if (table == null)
                throw new Exception("Invalid table");

            return await _context.MenuItems
                .Include(x => x.Category)
                .Where(x => x.Category.RestaurantId == table.RestaurantId)
                .Select(x => new MenuItemDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Price = x.Price,
                    CategoryId = x.CategoryId,
                    IsAvailable = x.IsAvailable,
                    ImageUrl = x.ImageUrl
                })
                .ToListAsync();
        }
    }
}
