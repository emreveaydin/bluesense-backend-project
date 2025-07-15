using Core.Domain.Entities;

namespace Core.Domain.Contracts;

public interface IGroupRepository : IGenericRepository<Group>
{
    // Example of a custom method we might need later
    Task<IEnumerable<Group>> GetGroupsForUserAsync(Guid userId);
    Task<Group?> GetByIdWithMembersAsync(Guid id);
} 