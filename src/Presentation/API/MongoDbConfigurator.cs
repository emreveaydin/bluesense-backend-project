namespace API;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

public static class MongoDbConfigurator
{
    private static readonly object Lock = new();
    private static bool _isConfigured;

    public static void Configure()
    {
        lock (Lock)
        {
            if (!_isConfigured)
            {
                BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
                _isConfigured = true;
            }
        }
    }
} 