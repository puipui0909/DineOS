using DineOS.Application.Common.Interfaces;
using DineOS.Application.DTOs;
using DineOS.Api.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DineOS.Api.Services
{
    public class SignalRRealtimeService : IRealtimeService
    {
        private readonly IHubContext<OrderHub> _hubContext;

        public SignalRRealtimeService(IHubContext<OrderHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotifyNewItems(string tableName, string message, Guid orderId)
        {
            await _hubContext.Clients.All.SendAsync("NotifyNewItems", new
            {
                tableName,
                message,
                orderId
            });
        }

        public async Task BroadcastOrderUpdated(OrderResponse order)
        {
            await _hubContext.Clients.All.SendAsync("OrderUpdated", order);
        }
        public async Task BroadcastOrderCreated(OrderResponse order)
        {
            await _hubContext.Clients.All
                .SendAsync("OrderCreated", order);
        }
    }
}