using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Domain.Enums
{
    public enum OrderStatus
    {
        Open = 1, 
        InProgress = 2,// Đang thêm món
        Closed = 3,      // Đã chốt món (chờ thanh toán)
        Paid = 4,        // Đã thanh toán xong
        Cancelled = 5    // Bị huỷ khi còn Open
    }
}
