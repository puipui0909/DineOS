using DineOS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DineOS.Application.Common.Interfaces
{
    public interface IRealtimeService
    {
        Task NotifyNewItems(string tableName, string message, Guid orderId);
        Task BroadcastOrderUpdated(OrderResponse order);
        Task BroadcastOrderCreated(OrderResponse order);
    }
}
