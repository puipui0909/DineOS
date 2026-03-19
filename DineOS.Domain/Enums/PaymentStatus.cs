using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Domain.Enums
{
    public enum PaymentStatus
    {
        Pending = 1,   // Tạo payment nhưng chưa xác nhận
        Paid = 2,      // Thanh toán thành công
        Failed = 3     // Thanh toán thất bại
    }
}