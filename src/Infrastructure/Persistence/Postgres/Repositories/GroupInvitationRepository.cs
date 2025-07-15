using Core.Domain.Contracts;
using Core.Domain.Entities;
using Infrastructure.Persistence.Postgres.Context;
using Infrastructure.Persistence.Postgres.Repositories;

namespace Infrastructure.Persistence.Postgres.Repositories;

public class GroupInvitationRepository : GenericRepository<GroupInvitation>, IGroupInvitationRepository
{
    public GroupInvitationRepository(ApplicationDbContext context) : base(context)
    {
    }
} 