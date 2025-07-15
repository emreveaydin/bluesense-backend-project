using Core.Domain.Entities;

namespace Core.Domain.Contracts;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
} 