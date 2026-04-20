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

        public async Task AddItemAsync(Guid orderId, AddOrderItemRequest request)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found");

            var menuItem = await _context.MenuItems.FindAsync(request.MenuItemId);
            if (menuItem == null)
                throw new Exception("Menu item not found");

            order.AddItem(
                request.MenuItemId,
                request.Quantity,
                menuItem.Price
            );

            await _context.SaveChangesAsync();
        }


        // 4️⃣ Xóa món
        public async Task RemoveItemAsync(Guid orderId, Guid orderItemId)
        {
            var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found");

            order.RemoveItem(orderItemId);
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
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found.");

            order.Close();
            await _context.SaveChangesAsync();
        }
        public async Task SendToKitchenAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new Exception("Order not found.");

            order.SendToKitchen();

            await _context.SaveChangesAsync();
        }

        // 6️⃣ Hủy order
        public async Task CancelAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            order.Cancel();
            await _context.SaveChangesAsync();
        }

        public async Task<OrderResponse?> GetByIdAsync(Guid orderId)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Payment)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null) return null;
            return new OrderResponse
            {
                Id = order.Id,
                Status = order.Status.ToString(),
                IsActive = order.IsActive,
                CreatedAt = order.CreatedAt,
                PaymentMethod = order.Payment != null
                ? order.Payment.Method.ToString()
                : null,

                Table = new TableDto
                {
                    Id = order.Table.Id,
                    Name = $"Bàn {order.Table.TableNumber}",
                    Status = order.Table.Status.ToString(),
                },
                Items = order.OrderItems.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    Name = i.MenuItem != null ? i.MenuItem.Name : "UNKNOWN",
                    Quantity = i.Quantity,
                    Price = i.UnitPrice,
                    CreatedAt = i.CreatedAt,
                    IsSentToKitchen = i.IsSentToKitchen,
                }).ToList(),
                Total = order.TotalAmount
            };
        }

        public async Task<OrderResponse> GetOrCreateByTableAsync(Guid tableId)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.MenuItem)
                .Where(o => o.TableId == tableId && o.IsActive)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                var table = await _context.Tables
                    .FirstOrDefaultAsync(t => t.Id == tableId);

                if (table == null)
                    throw new Exception("Table not found");

                if (table.Status != TableStatus.Occupied)
                {
                    table.MarkAsOccupied();
                }

                order = table.CreateOrder();

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // reload navigation
                order = await _context.Orders
                    .Include(o => o.Table)
                    .Include(o => o.OrderItems)
                        .ThenInclude(i => i.MenuItem)
                    .FirstAsync(o => o.Id == order.Id);
            }

            return new OrderResponse
            {
                Id = order.Id,
                Status = order.Status.ToString(),
                IsActive = order.IsActive,
                Table = new TableDto
                {
                    Id = order.Table.Id,
                    Name = $"Bàn {order.Table.TableNumber}",
                    Status = order.Table.Status.ToString()
                },

                Items = order.OrderItems.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    Name = i.MenuItem != null ? i.MenuItem.Name : "UNKNOWN",
                    Quantity = i.Quantity,
                    Price = i.UnitPrice,
                    CreatedAt = i.CreatedAt,
                    IsSentToKitchen = i.IsSentToKitchen,
                }).ToList(),

                Total = order.TotalAmount
            };
        }

        public async Task<List<OrderResponse>> GetAllAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.MenuItem)
                .ToListAsync();

            return orders.Select(order => new OrderResponse
            {
                Id = order.Id,
                Status = order.Status.ToString(),
                IsActive = order.IsActive,
                Table = new TableDto
                {
                    Id = order.Table.Id,
                    Name = $"Bàn {order.Table.TableNumber}",
                    Status = order.Table.Status.ToString()
                },

                Items = order.OrderItems.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    Name = i.MenuItem != null ? i.MenuItem.Name : "UNKNOWN",
                    Quantity = i.Quantity,
                    Price = i.UnitPrice,
                    IsSentToKitchen = i.IsSentToKitchen,
                    CreatedAt = i.CreatedAt
            
                }).ToList(),

                Total = order.TotalAmount
            }).ToList();
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

        public async Task<List<OrderHistoryDto>> GetOrderHistoryAsync(string? search, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Orders
                .Include(o => o.Table)
                .Include(o => o.Payment)
                .Include(o => o.OrderItems) // fix luôn count
                .AsQueryable();

            query = query.Where(o => o.Status == OrderStatus.Paid);

            if (fromDate.HasValue)
                query = query.Where(o => o.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(o => o.CreatedAt <= toDate.Value);

            if (!string.IsNullOrEmpty(search))
            {
                var normalized = search.Replace("#", "").ToLower();

                query = query.Where(o =>
                    o.Id.ToString().Replace("-", "").ToLower().StartsWith(normalized)
                );
            }

            // ❗ STEP 1: Query raw data (KHÔNG ToString)
            var data = await query
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new
                {
                    o.Id,
                    o.CreatedAt,
                    TableNumber = o.Table != null ? o.Table.TableNumber : (int?)null,
                    Items = o.OrderItems.Count,
                    o.Status,
                    PaymentMethod = o.Payment != null ? o.Payment.Method : (PaymentMethod?)null,
                    o.TotalAmount
                })
                .ToListAsync();

            // ❗ STEP 2: Map sang DTO (ToString ở memory)
            return data.Select(o => new OrderHistoryDto
            {
                Id = o.Id,
                OrderNo = "#" + o.Id.ToString().Replace("-", "").Substring(0, 4),
                TableName = o.TableNumber != null ? $"T-{o.TableNumber}" : "N/A",
                CreatedAt = o.CreatedAt,
                Items = o.Items,
                Status = o.Status.ToString(),
                PaymentMethod = o.PaymentMethod != null
                    ? o.PaymentMethod.ToString()
                    : "N/A", // ✅ SAFE
                TotalAmount = o.TotalAmount
            }).ToList();
        }
    }
}
