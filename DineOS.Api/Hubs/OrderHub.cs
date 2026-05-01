using DineOS.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization; // Thêm namespace này
using Microsoft.AspNetCore.SignalR;

namespace DineOS.Api.Hubs
{
    public class OrderHub : Hub<IOrderHubClient>
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }
    }
}