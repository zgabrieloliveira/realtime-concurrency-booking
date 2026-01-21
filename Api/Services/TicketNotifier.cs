using Api.Hubs;
using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Api.Services;

public class TicketNotifier : ITicketNotifier
{
    private readonly IHubContext<TicketHub> _hubContext;

    public TicketNotifier(IHubContext<TicketHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifySeatLockedAsync(string seatId)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveSeatUpdate", seatId, "LOCKED");
    }
    
    public async Task NotifySeatSoldAsync(string seatId)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveSeatUpdate", seatId, "SOLD");
    }
    
}