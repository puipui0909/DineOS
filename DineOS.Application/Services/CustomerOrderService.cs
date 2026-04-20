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
    public class CustomerOrderService : ICustomerOrderService
    {
        private readonly IApplicationDbContext _context;

        public CustomerOrderService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderResponse?> GetByTableAsync(Guid tableId)
        {
            var order = await _context.Orders
            .Where(o => o.TableId == tableId && o.IsActive)
            .OrderByDescending(o => o.CreatedAt)
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.MenuItem)
            .Include(o => o.Table)
            .FirstOrDefaultAsync();

            if (order == null)
                return null;

            return Map(order);
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request)
        {
            if (request.Items == null || !request.Items.Any())
                throw new Exception("Order must have at least one item");

            var table = await _context.Tables.FindAsync(request.TableId);
            if (table == null)
                throw new Exception("Table not found");

            // 🔥 LẤY ORDER NẾU ĐÃ CÓ
            var order = await _context.Orders
                .Where(o => o.TableId == request.TableId && o.IsActive)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync();

            // 🔥 CHƯA CÓ → CREATE
            if (order == null)
            {
                table.MarkAsOccupied();

                order = table.CreateOrder();
                _context.Orders.Add(order);
            }

            // 🔥 LOAD MENU ITEMS 1 LẦN
            var menuItemIds = request.Items.Select(i => i.MenuItemId).ToList();

            var menuItems = await _context.MenuItems
                .Where(m => menuItemIds.Contains(m.Id))
                .ToDictionaryAsync(m => m.Id);

            foreach (var item in request.Items)
            {
                if (item.Quantity <= 0)
                    throw new Exception("Invalid quantity");

                if (!menuItems.TryGetValue(item.MenuItemId, out var menuItem))
                    throw new Exception("MenuItem not found");

                order.AddItem(
                    item.MenuItemId,
                    item.Quantity,
                    menuItem.Price
                );
            }

            await _context.SaveChangesAsync();

            return Map(order);
        }

        // =========================
        // MAPPER
        // =========================
        private OrderResponse Map(Order order)
        {
            return new OrderResponse
            {
                Id = order.Id,
                Status = order.Status.ToString(),
                IsActive = order.IsActive,
                CreatedAt = order.CreatedAt,
                Table = new TableDto
                {
                    Id = order.Table.Id,
                    Name = $"Bàn {order.Table.TableNumber}",
                    Status = order.Table.Status.ToString()
                },
                Items = order.OrderItems.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    Name = i.MenuItem?.Name ?? "UNKNOWN",
                    Quantity = i.Quantity,
                    Price = i.UnitPrice,
                    CreatedAt = i.CreatedAt,
                    IsSentToKitchen = i.IsSentToKitchen
                }).ToList(),
                Total = order.TotalAmount
            };
        }
    }
}
