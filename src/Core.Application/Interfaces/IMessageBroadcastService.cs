using Core.Application.DTOs;
using System.Threading.Tasks;

namespace Core.Application.Interfaces;

public interface IMessageBroadcastService
{
    Task BroadcastMessageAsync(MessageDto message);
    Task MessageEdited(Guid groupId, MessageDto updatedMessage);
    Task MessageDeleted(Guid groupId, Guid messageId);
} 