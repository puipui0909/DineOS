using DineOS.Application.DTOs;
using DineOS.Domain.Entities;
using DineOS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.Common.Interfaces
{
    public interface IOrderService
    {
        Task<Guid> OpenTableAsync(Guid tableId);
        Task AddItemAsync(Guid orderId, Guid menuItemId, int quantity);
        Task RemoveItemAsync(Guid orderItemId);
        Task CancelAsync(Guid orderId);
        Task CloseAsync(Guid orderId);
        Task<Order?> GetByIdAsync(Guid orderId);
        Task<Order?> GetByTableAsync(Guid tableId);
        Task<List<Order>> GetAllAsync();
        Task<BillResponse?> GetBillAsync(Guid orderId);
        Task<List<Order>> GetOrderHistoryAsync();
    }

}
