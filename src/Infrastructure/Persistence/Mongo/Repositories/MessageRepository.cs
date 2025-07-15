using Core.Domain.Contracts;
using Core.Domain.Entities;
using Infrastructure.Persistence.Mongo.Context;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Mongo.Repositories;

public class MessageRepository : IMessageRepository
{
    private readonly MongoDbContext _context;

    public MessageRepository(MongoDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Message entity)
    {
        await _context.Messages.InsertOneAsync(entity);
    }

    public async Task<Message?> GetByIdAsync(Guid id)
    {
        return await _context.Messages.Find(m => m.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Message>> GetMessagesForGroupAsync(Guid groupId, int pageNumber, int pageSize)
    {
        return await _context.Messages
            .Find(m => m.GroupId == groupId && !m.IsDeleted)
            .SortByDescending(m => m.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public async Task<IEnumerable<Message>> SearchMessagesInGroupAsync(Guid groupId, string searchText, int pageNumber, int pageSize)
    {
        var filter = Builders<Message>.Filter.And(
            Builders<Message>.Filter.Eq(m => m.GroupId, groupId),
            Builders<Message>.Filter.Eq(m => m.IsDeleted, false),
            Builders<Message>.Filter.Text(searchText)
        );

        return await _context.Messages
            .Find(filter)
            .SortByDescending(m => m.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();
    }

    public void Update(Message entity)
    {
        // In MongoDB, updates are typically done via ReplaceOne or UpdateOne.
        _context.Messages.ReplaceOne(m => m.Id == entity.Id, entity);
    }

    // The following methods from IGenericRepository are not directly applicable
    // in the same way for MongoDB or are covered by the specific methods above.
    // We provide placeholder or logical equivalent implementations.

    public Task<IEnumerable<Message>> GetAllAsync()
    {
        throw new NotImplementedException("GetAllAsync is not supported for messages. Use GetMessagesForGroupAsync.");
    }

    public Task<IEnumerable<Message>> FindAsync(Expression<Func<Message, bool>> predicate)
    {
        // This could be implemented, but specific queries are preferred.
        return _context.Messages.Find(predicate).ToListAsync().ContinueWith(t => t.Result as IEnumerable<Message>);
    }

    public Task AddRangeAsync(IEnumerable<Message> entities)
    {
        return _context.Messages.InsertManyAsync(entities);
    }

    public void Remove(Message entity)
    {
        // This would be a hard delete. Soft delete is handled in the entity itself.
        _context.Messages.DeleteOne(m => m.Id == entity.Id);
    }

    public Task<int> SaveChangesAsync()
    {
        // MongoDB driver does not have a Unit of Work / SaveChanges concept like EF Core.
        // Operations are sent to the database immediately.
        return Task.FromResult(0);
    }
} 