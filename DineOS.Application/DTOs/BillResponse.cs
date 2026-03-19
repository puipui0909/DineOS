using DineOS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.DTOs
{
    public class BillResponse
    {
        public int TableNumber { get; set; }
        public List<BillItem> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public DateTime? PaidAt { get; set; }
    }

    public class BillItem
    {
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Price * Quantity;
    }
}
