using DineOS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Collections.Generic;

namespace DineOS.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        // Phải dùng DbSet để hỗ trợ Async và các thao tác Add/Update
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<Table> Tables { get; }
        DbSet<MenuItem> MenuItems { get; }
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }
        DatabaseFacade Database { get; }
        DbSet<Payment> Payments { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}