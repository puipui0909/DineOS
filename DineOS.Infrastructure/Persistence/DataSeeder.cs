using DineOS.Domain.Entities; // Giả định namespace chứa Entity của bạn
using Microsoft.EntityFrameworkCore;

namespace DineOS.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(DineOSDbContext context)
    {
        // --- SEED ROLE ---
        if (!await context.Roles.AnyAsync())
        {
            var adminRole = new Role("Admin");
            var staffRole = new Role("Staff");

            await context.Roles.AddRangeAsync(adminRole, staffRole);
            await context.SaveChangesAsync();
        }

        // --- SEED RESTAURANT ---
        if (!await context.Restaurants.AnyAsync())
        {
            var restaurant = new Restaurant(
                "DineOS Premium Restaurant",
                "123 Tech Street, Digital City"
            );

            await context.Restaurants.AddAsync(restaurant);
            await context.SaveChangesAsync();
        }

        var restaurantId = await context.Restaurants
            .Select(r => r.Id)
            .FirstAsync();

        // --- SEED USER ---
        if (!await context.Users.AnyAsync())
        {
            var adminRoleId = await context.Roles
                .Where(r => r.Name == "Admin")
                .Select(r => r.Id)
                .FirstAsync();

            var staffRoleId = await context.Roles
                .Where(r => r.Name == "Staff")
                .Select(r => r.Id)
                .FirstAsync();

            var users = new List<User>
            {
                new User("admin", "admin123", adminRoleId, restaurantId),
                new User("staff1", "staff123", staffRoleId, restaurantId),
                new User("staff2", "staff123", staffRoleId, restaurantId)
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
        // --- SEED TABLES ---
        if (!await context.Tables.AnyAsync())
        {
            var tables = new List<Table>
        {
            new Table(1, restaurantId),
            new Table(2, restaurantId),
            new Table(3, restaurantId)
        };

            await context.Tables.AddRangeAsync(tables);
            await context.SaveChangesAsync();
        }

        // --- SEED CATEGORIES ---
        if (!await context.Categories.AnyAsync())
        {
            var categoryFood = new Category("Món Chính", restaurantId);
            var categoryDrink = new Category("Đồ Uống", restaurantId);
            var categoryDessert = new Category("Tráng Miệng", restaurantId);

            await context.Categories.AddRangeAsync(categoryFood, categoryDrink, categoryDessert);
            await context.SaveChangesAsync();
        }

        // --- SEED MENU ITEMS ---
        if (!await context.MenuItems.AnyAsync())
        {
            var categoryFood = await context.Categories.FirstAsync(c => c.Name == "Món Chính");
            var categoryDrink = await context.Categories.FirstAsync(c => c.Name == "Đồ Uống");
            var categoryDessert = await context.Categories.FirstAsync(c => c.Name == "Tráng Miệng");

            var menuItems = new List<MenuItem>
        {
            new MenuItem("Cơm Chiên Dương Châu", 55000, categoryFood.Id),
            new MenuItem("Phở Bò Đặc Biệt", 65000, categoryFood.Id),
            new MenuItem("Bún Bò Huế", 60000, categoryFood.Id),

            new MenuItem("Trà Đào Cam Sả", 35000, categoryDrink.Id),
            new MenuItem("Cà Phê Sữa Đá", 25000, categoryDrink.Id),
            new MenuItem("Nước Chanh", 20000, categoryDrink.Id),

            new MenuItem("Bánh Flan", 30000, categoryDessert.Id),
            new MenuItem("Kem Vanilla", 35000, categoryDessert.Id)
        };

            await context.MenuItems.AddRangeAsync(menuItems);
            await context.SaveChangesAsync();
        }
    }
}