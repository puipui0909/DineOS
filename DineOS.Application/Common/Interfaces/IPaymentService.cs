using DineOS.Domain.Entities;
using DineOS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.Common.Interfaces
{
    public interface IPaymentService
    {
        Task PayAsync(Guid orderId, PaymentMethod method);
        Task<Payment?> GetByOrderIdAsync(Guid orderId);
        Task<Payment?> GetByIdAsync(Guid paymentId);
    }
}
