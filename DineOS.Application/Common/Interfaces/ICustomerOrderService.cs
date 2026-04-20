using DineOS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.Common.Interfaces
{
    public interface ICustomerOrderService
    {
        Task<OrderResponse?> GetByTableAsync(Guid tableId);
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request);
    }
}
