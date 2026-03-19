using DineOS.Application.Common.Interfaces;
using DineOS.Application.DTOs;
using DineOS.Domain.Entities;
using DineOS.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DineOS.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IApplicationDbContext _context;

        public OrderService(IApplicationDbContext context)
        {
            _context = context;
        }

        // 1️⃣ Mở bàn
        public async Task<Guid> OpenTableAsync(Guid tableId)
        {
            var table = await _context.Tables
                .FirstOrDefaultAsync(t => t.Id == tableId);

            if (table == null)
                throw new Exception("Table not found.");

            if (table.Status != TableStatus.Available)
                throw new Exception("Table is not available.");

            table.MarkAsOccupied();

            var order = Order.Create(tableId);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return order.Id;
        }

        // 2️⃣ Thêm món
        public async Task AddItemAsync(Guid orderId, Guid menuItemId, int quantity)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than 0.");

            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found.");

            if (order.Status != OrderStatus.Open)
                throw new Exception("Order is not open.");

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(m => m.Id == menuItemId);

            if (menuItem == null)
                throw new Exception("Menu item not found.");

            // 👉 GỌI DOMAIN
            order.AddItem(menuItem.Id, quantity, menuItem.Price);
            var dbContext = (DbContext)_context;
            await _context.SaveChangesAsync();
        }

        // 4️⃣ Xóa món
        public async Task RemoveItemAsync(Guid orderItemId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o =>
                    o.OrderItems.Any(i => i.Id == orderItemId));

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            order.RemoveItemById(orderItemId);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new Exception("Concurrency conflict detected.");
            }
        }

        public async Task CloseAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found.");

            order.Close();

            await _context.SaveChangesAsync();
        }

        // 6️⃣ Hủy order
        public async Task CancelAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found.");

            try
            {
                order.Cancel();
            }
            catch (InvalidOperationException ex)
            {
                throw new ApplicationException(ex.Message);
            }

            order.Table!.MarkAsAvailable();

            await _context.SaveChangesAsync();
        }

        public async Task<Order?> GetByIdAsync(Guid orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<Order?> GetByTableAsync(Guid tableId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.TableId == tableId && o.Status == OrderStatus.Open);
        }

        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                .ToListAsync();
        }

        public async Task<BillResponse?> GetBillAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.MenuItem)
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return null;

            var bill = new BillResponse
            {
                TableNumber = order.Table.TableNumber,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.Payment?.Method,
                PaidAt = order.Payment?.PaidAt
            };

            foreach (var item in order.OrderItems)
            {
                bill.Items.Add(new BillItem
                {
                    Name = item.MenuItem.Name,
                    Quantity = item.Quantity,
                    Price = item.UnitPrice
                });
            }

            return bill;
        }

        public async Task<List<Order>> GetOrderHistoryAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == OrderStatus.Closed)
                .Include(o => o.Table)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
    }
}
