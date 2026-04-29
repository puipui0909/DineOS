using DineOS.Application.Common.Interfaces;
using DineOS.Domain.Entities;
using DineOS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IApplicationDbContext _context;

        public PaymentService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task PayAsync(Guid orderId, PaymentMethod method)
        {
            var order = await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            if (order.Payment != null)
                throw new InvalidOperationException("Order already paid.");

            if (order.Status != OrderStatus.Closed)
                throw new InvalidOperationException("Order must be closed before payment.");

            // 1️⃣ xử lý order
            order.CompletePayment(method);

            _context.Payments.Add(order.Payment);

            // 2️⃣ xử lý table riêng (KEY FIX)
            var table = await _context.Tables
                .FirstOrDefaultAsync(t => t.Id == order.TableId);

            if (table == null)
                throw new Exception("Table not found");

            table.MarkAsAvailable();

            // 3️⃣ save
            await _context.SaveChangesAsync();
        }
        public async Task<Payment?> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(x => x.OrderId == orderId);
        }

        public async Task<Payment?> GetByIdAsync(Guid paymentId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(x => x.Id == paymentId);
        }
    }
}
