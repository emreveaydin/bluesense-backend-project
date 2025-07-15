using Core.Domain.Entities;

namespace Core.Domain.Contracts;

public interface IMessageRepository : IGenericRepository<Message>
{
    Task<IEnumerable<Message>> GetMessagesForGroupAsync(Guid groupId, int pageNumber, int pageSize);
    Task<IEnumerable<Message>> SearchMessagesInGroupAsync(Guid groupId, string searchText, int pageNumber, int pageSize);
} 