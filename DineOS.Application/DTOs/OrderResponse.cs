using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.DTOs
{
    public class OrderResponse
    {
        public Guid Id { get; set; }
        public string Status { get; set; }

        public TableDto Table { get; set; }

        public List<OrderItemDto> Items { get; set; } = new();

        public bool IsActive { get; set; }

        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
