using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public async Task JoinGroup(string groupId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
    }

    public async Task LeaveGroup(string groupId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupId);
    }

    // A client can call this method to send a message.
    // However, we will handle message creation via our API Controller
    // and use IHubContext to broadcast the message from the backend.
    // This method can be used for other real-time interactions if needed.
    public async Task SendMessageToGroup(string groupId, string user, string message)
    {
        await Clients.Group(groupId).SendAsync("ReceiveMessage", user, message);
    }
} 