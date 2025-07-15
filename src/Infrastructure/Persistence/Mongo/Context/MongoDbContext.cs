using Core.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Persistence.Mongo.Context;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);

        // Create Text Index for Message Content
        var messageCollection = _database.GetCollection<Message>(GetCollectionName<Message>());
        var indexKeysDefinition = Builders<Message>.IndexKeys.Text(m => m.Content);
        messageCollection.Indexes.CreateOne(new CreateIndexModel<Message>(indexKeysDefinition));
    }

    public IMongoCollection<Message> Messages => _database.GetCollection<Message>(GetCollectionName<Message>());
    
    private string GetCollectionName<T>()
    {
        return typeof(T).Name + "s";
    }
} 