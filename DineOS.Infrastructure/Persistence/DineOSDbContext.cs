using Microsoft.EntityFrameworkCore;
using DineOS.Domain.Entities;
using DineOS.Application.Common.Interfaces;

namespace DineOS.Infrastructure.Persistence
{
    public class DineOSDbContext : DbContext, IApplicationDbContext
    {
        public DineOSDbContext(DbContextOptions<DineOSDbContext> options)  : base(options)
        {
        }

        public DbSet<Restaurant> Restaurants => Set<Restaurant>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<MenuItem> MenuItems => Set<MenuItem>();
        public DbSet<Table> Tables => Set<Table>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(DineOSDbContext).Assembly);
        }
    }
}
