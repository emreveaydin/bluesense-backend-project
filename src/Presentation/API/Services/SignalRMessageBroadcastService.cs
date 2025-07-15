using API.Hubs;
using Core.Application.DTOs;
using Core.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace API.Services;

public class SignalRMessageBroadcastService : IMessageBroadcastService
{
    private readonly IHubContext<ChatHub> _hubContext;

    public SignalRMessageBroadcastService(IHubContext<ChatHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task BroadcastMessageAsync(MessageDto message)
    {
        await _hubContext.Clients.Group(message.GroupId.ToString()).SendAsync("ReceiveMessage", message);
    }

    public async Task MessageEdited(Guid groupId, MessageDto updatedMessage)
    {
        await _hubContext.Clients.Group(groupId.ToString()).SendAsync("UpdateMessage", updatedMessage);
    }

    public async Task MessageDeleted(Guid groupId, Guid messageId)
    {
        await _hubContext.Clients.Group(groupId.ToString()).SendAsync("DeleteMessage", messageId);
    }
} 