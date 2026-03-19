using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DineOS.Domain.Enums;

namespace DineOS.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; private set; }
        // Foreign key
        public Guid OrderId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentMethod Method { get; private set; }
        public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
        public DateTime? PaidAt { get; private set; }

        // Navigation
        public Order Order { get; private set; } = null!;

        private Payment() { }
        internal Payment(Order order, PaymentMethod method)
        {
            Id = Guid.NewGuid();
            OrderId = order.Id;
            Amount = order.TotalAmount;
            Method = method;
            Status = PaymentStatus.Pending;
        }

        public void MarkAsPaid()
        {
            if (Status == PaymentStatus.Paid)
                throw new InvalidOperationException("Payment đã hoàn tất.");

            Status = PaymentStatus.Paid;
            PaidAt = DateTime.UtcNow;
        }

        public void MarkAsFailed()
        {
            if (Status == PaymentStatus.Paid)
                throw new InvalidOperationException("Không thể fail sau khi đã thanh toán.");

            Status = PaymentStatus.Failed;
        }

    }
}
