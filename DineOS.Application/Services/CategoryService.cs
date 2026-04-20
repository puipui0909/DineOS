using DineOS.Application.Common.Interfaces;
using DineOS.Application.DTOs;
using DineOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace DineOS.Application.Services
{
    public class CategoryService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public CategoryService(
            IApplicationDbContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<CategoryDto>> GetAllAsync()
        {
            var restaurantId = _currentUserService.GetRestaurantId();

            return await _context.Categories
                .Where(c => c.RestaurantId == restaurantId)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        // 📌 Create category
        public async Task<Guid> CreateAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên category không hợp lệ");

            var restaurantId = _currentUserService.GetRestaurantId();

            var category = new Category(name, restaurantId);

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return category.Id;
        }

        // 📌 Delete
        public async Task DeleteAsync(Guid id)
        {
            var restaurantId = _currentUserService.GetRestaurantId();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id && c.RestaurantId == restaurantId);

            if (category == null)
                throw new KeyNotFoundException("Category không tồn tại");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        //for customers
        public async Task<List<CategoryDto>> GetByTableAsync(Guid tableId)
        {
            // Bước 1: Truy vết từ TableId ra RestaurantId
            var restaurantId = await _context.Tables
                .Where(t => t.Id == tableId)
                .Select(t => t.RestaurantId)
                .FirstOrDefaultAsync();

            if (restaurantId == Guid.Empty)
                throw new KeyNotFoundException("Bàn không hợp lệ hoặc không tồn tại.");

            // Bước 2: Lấy dữ liệu Category của nhà hàng đó
            return await _context.Categories
                .Where(c => c.RestaurantId == restaurantId)
                .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
                .ToListAsync();
        }
        
    }
}