using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.DTOs
{
    public class AddItemRequest
    {
        public Guid MenuItemId { get; set; }
        public int Quantity { get; set; }
        // Bạn có thể thêm Note (ghi chú món ăn) tại đây sau này
    } 
}
