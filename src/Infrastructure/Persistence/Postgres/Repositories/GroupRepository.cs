using Core.Domain.Contracts;
using Core.Domain.Entities;
using Infrastructure.Persistence.Postgres.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Postgres.Repositories;

public class GroupRepository : GenericRepository<Group>, IGroupRepository
{
    public GroupRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Group>> GetGroupsForUserAsync(Guid userId)
    {
        return await _context.Groups
            .Where(g => g.GroupMembers.Any(gm => gm.UserId == userId))
            .ToListAsync();
    }

    public async Task<Group?> GetByIdWithMembersAsync(Guid id)
    {
        return await _context.Groups
            .Include(g => g.GroupMembers)
            .FirstOrDefaultAsync(g => g.Id == id);
    }
} 