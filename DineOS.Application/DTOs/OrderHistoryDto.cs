using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.DTOs
{
    public class OrderHistoryDto
    {
        public Guid Id { get; set; }

        public string OrderNo { get; set; }

        public string TableName { get; set; }

        public DateTime CreatedAt { get; set; }

        public int Items { get; set; }

        public string Status { get; set; }

        public string PaymentMethod { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
