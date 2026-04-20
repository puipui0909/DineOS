using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.DTOs
{
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsSentToKitchen { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
