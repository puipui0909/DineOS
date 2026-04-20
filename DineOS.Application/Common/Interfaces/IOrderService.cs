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
        Task AddItemAsync(Guid orderId, AddOrderItemRequest request);
        Task RemoveItemAsync(Guid orderId, Guid orderItemId);
        Task CancelAsync(Guid orderId);
        Task CloseAsync(Guid orderId);
        Task<OrderResponse?> GetByIdAsync(Guid orderId);
        Task<OrderResponse> GetOrCreateByTableAsync(Guid tableId);
        Task<List<OrderResponse>> GetAllAsync();
        Task<BillResponse?> GetBillAsync(Guid orderId);
        Task<List<OrderHistoryDto>> GetOrderHistoryAsync(string? search, DateTime? fromDate, DateTime? toDate);
        Task SendToKitchenAsync(Guid orderId);
    }

}
