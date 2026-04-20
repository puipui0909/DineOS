using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.DTOs
{
    public class DashboardSummaryDto
    {
        public int TotalOrders { get; set; }
        public int ActiveTables { get; set; }
        public int TotalTables { get; set; }
        public decimal RevenueToday { get; set; }
    }
}
