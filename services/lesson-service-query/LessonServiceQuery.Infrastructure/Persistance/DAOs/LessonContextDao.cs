using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Infrastructure.Configuration;
using LessonServiceQuery.Infrastructure.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LessonServiceQuery.Infrastructure.Persistance.DAOs;

public class LessonContextDao : ILessonContextDao
{
    private readonly IMongoCollection<LessonContext> _lessonContexts;
    
    public LessonContextDao(IMongoDbContext context, IOptions<MongoDbSettings> settings)
    {
        _lessonContexts = context.GetCollection<LessonContext>(settings.Value.LessonContextsCollectionName);
    }
    
    public async Task<LessonContext?> GetByIdAsync(Guid lessonContextId)
    {
        return await _lessonContexts.Find(x => x.LessonContextId == lessonContextId && x.IsActive)
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<LessonContext>> GetByLessonIdAsync(Guid lessonId)
    {
        return await _lessonContexts.Find(x => x.LessonId == lessonId && x.IsActive)
            .SortBy(x => x.Position)
            .ToListAsync();
    }
    
    public async Task<LessonContext> CreateAsync(Guid lessonId, LessonContext lessonContext)
    {
        lessonContext.LessonId = lessonId;
        lessonContext.CreatedAt = DateTime.UtcNow;
        lessonContext.UpdatedAt = DateTime.UtcNow;
        lessonContext.IsActive = true;
        
        await _lessonContexts.InsertOneAsync(lessonContext);
        return lessonContext;
    }
    
    public async Task<LessonContext> UpdateAsync(LessonContext lessonContext)
    {
        lessonContext.UpdatedAt = DateTime.UtcNow;
        await _lessonContexts.ReplaceOneAsync(
            x => x.LessonContextId == lessonContext.LessonContextId && x.IsActive,
            lessonContext);
        return lessonContext;
    }
    
    public async Task DeleteAsync(Guid lessonContextId)
    {
        // Soft delete: set all versions to IsActive = false
        await DeactivateAllByIdAsync(lessonContextId);
    }
    
    public async Task<bool> ExistsAsync(Guid lessonContextId)
    {
        return await _lessonContexts.CountDocumentsAsync(
            x => x.LessonContextId == lessonContextId) > 0;
    }
    
    public async Task<bool> ExistsInLessonAsync(Guid lessonId, Guid lessonContextId)
    {
        return await _lessonContexts.CountDocumentsAsync(
            x => x.LessonId == lessonId && x.LessonContextId == lessonContextId) > 0;
    }

    public async Task DeactivateAllByIdAsync(Guid lessonContextId)
    {
        var filter = Builders<LessonContext>.Filter.Eq(x => x.LessonContextId, lessonContextId);
        var update = Builders<LessonContext>.Update.Set(x => x.IsActive, false);
        await _lessonContexts.UpdateManyAsync(filter, update);
    }
}
