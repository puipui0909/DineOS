using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.DTOs
{
    public class AddOrderItemRequest
    {
        public Guid MenuItemId { get; set; }
        public int Quantity { get; set; }
    }
}
