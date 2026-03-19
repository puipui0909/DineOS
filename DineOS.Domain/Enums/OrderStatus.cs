using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Domain.Enums
{
    public enum OrderStatus
    {
        Open = 1,        // Đang thêm món
        Closed = 2,      // Đã chốt món (chờ thanh toán)
        Paid = 3,        // Đã thanh toán xong
        Cancelled = 4    // Bị huỷ khi còn Open
    }
}
