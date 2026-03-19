using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Domain.Enums
{
    public enum TableStatus
    {
        Available = 0,    // Bàn trống
        Occupied = 1,     // Đang có khách
        Reserved = 2,     // Đã được đặt trước
        OutOfService = 3  // Đang bảo trì/hỏng
    }
}