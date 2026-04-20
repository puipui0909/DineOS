using DineOS.Application.Common.Interfaces;
using DineOS.Application.DTOs;
using DineOS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IApplicationDbContext _context;

        public DashboardService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var today = DateTime.Today;

            var totalOrders = await _context.Orders
                .CountAsync(o => o.CreatedAt.Date == today);

            var activeTables = await _context.Tables
                .CountAsync(t => t.Status == TableStatus.Occupied);

            var totalTables = await _context.Tables.CountAsync();

            var revenueToday = await _context.Orders
                .Where(o => o.Status == OrderStatus.Paid &&
                            o.CreatedAt.Date == today)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0;

            return new DashboardSummaryDto
            {
                TotalOrders = totalOrders,
                ActiveTables = activeTables,
                TotalTables = totalTables,
                RevenueToday = revenueToday
            };
        }

        public async Task<List<RevenueMonthlyDto>> GetRevenueMonthlyAsync(int year)
        {
            return await _context.Orders
                .Where(o => o.Status == OrderStatus.Paid &&
                            o.CreatedAt.Year == year)
                .GroupBy(o => o.CreatedAt.Month)
                .Select(g => new RevenueMonthlyDto
                {
                    Month = g.Key,
                    Total = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(x => x.Month)
                .ToListAsync();
        }
    }
}
