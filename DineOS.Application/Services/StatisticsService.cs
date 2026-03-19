using DineOS.Application.Common.Interfaces;
using DineOS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.Services
{
    public class StatisticsService
    {
        private readonly IApplicationDbContext _context;

        public StatisticsService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal> GetTodayRevenueAsync()
        {
            var today = DateTime.UtcNow.Date;

            return await _context.Payments
                .Where(p => p.Status == PaymentStatus.Paid &&
                            p.PaidAt >= today)
                .SumAsync(p => p.Amount);
        }
    }
}
