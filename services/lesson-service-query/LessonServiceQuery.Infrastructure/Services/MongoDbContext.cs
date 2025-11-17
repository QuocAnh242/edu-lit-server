using LessonServiceQuery.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LessonServiceQuery.Infrastructure.Services;

public interface IMongoDbContext
{
    IMongoDatabase Database { get; }
    IMongoCollection<T> GetCollection<T>(string name);
}

public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;
    
    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }
    
    public IMongoDatabase Database => _database;
    
    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}
