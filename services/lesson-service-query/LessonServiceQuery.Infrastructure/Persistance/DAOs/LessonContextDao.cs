using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Infrastructure.Configuration;
using LessonServiceQuery.Infrastructure.Services;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LessonServiceQuery.Infrastructure.Persistance.DAOs;

public class LessonContextDao : ILessonContextDao
{
    private readonly IMongoCollection<Lesson> _lessons;
    
    public LessonContextDao(IMongoDbContext context, IOptions<MongoDbSettings> settings)
    {
        _lessons = context.GetCollection<Lesson>(settings.Value.LessonsCollectionName);
    }
    
    public async Task<LessonContext?> GetByIdAsync(Guid lessonContextId)
    {
        var lesson = await _lessons
            .Find(l => l.IsActive && l.LessonContexts.Any(lc => lc.LessonContextId == lessonContextId && lc.IsActive))
            .FirstOrDefaultAsync();
            
        return lesson?.LessonContexts.FirstOrDefault(lc => lc.LessonContextId == lessonContextId && lc.IsActive);
    }
    
    public async Task<List<LessonContext>> GetByLessonIdAsync(Guid lessonId)
    {
        var lesson = await _lessons
            .Find(l => l.LessonId == lessonId && l.IsActive)
            .FirstOrDefaultAsync();
            
        return lesson?.LessonContexts.Where(lc => lc.IsActive).ToList() ?? new List<LessonContext>();
    }
    
    public async Task<LessonContext> CreateAsync(Guid lessonId, LessonContext lessonContext)
    {
        lessonContext.CreatedAt = DateTime.UtcNow;
        lessonContext.UpdatedAt = DateTime.UtcNow;
        
        // Check if lesson exists
        var lessonExists = await _lessons.CountDocumentsAsync(l => l.LessonId == lessonId) > 0;
        if (!lessonExists)
        {
            throw new InvalidOperationException($"Lesson with ID {lessonId} not found");
        }
        
        // Check if lesson context already exists in the lesson
        var existsInLesson = await ExistsInLessonAsync(lessonId, lessonContext.LessonContextId);
        if (existsInLesson)
        {
            throw new InvalidOperationException($"LessonContext with ID {lessonContext.LessonContextId} already exists in lesson {lessonId}");
        }
        
        // Add the lesson context to the lesson's lesson_contexts array
        var filter = Builders<Lesson>.Filter.Eq(l => l.LessonId, lessonId);
        var update = Builders<Lesson>.Update.Push(l => l.LessonContexts, lessonContext);
        
        var result = await _lessons.UpdateOneAsync(filter, update);
        
        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Failed to add LessonContext to Lesson with ID {lessonId}");
        }
        
        return lessonContext;
    }
    
    public async Task<LessonContext> UpdateAsync(LessonContext lessonContext)
    {
        lessonContext.UpdatedAt = DateTime.UtcNow;
        
        var filter = Builders<Lesson>.Filter.ElemMatch(l => l.LessonContexts, 
            lc => lc.LessonContextId == lessonContext.LessonContextId);
            
        var update = Builders<Lesson>.Update.Set("lesson_contexts.$", lessonContext);
        
        var result = await _lessons.UpdateOneAsync(filter, update);
        
        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"LessonContext with ID {lessonContext.LessonContextId} not found");
        }
        
        return lessonContext;
    }
    
    public async Task DeleteAsync(Guid lessonContextId)
    {
        var filter = Builders<Lesson>.Filter.Empty;
        var update = Builders<Lesson>.Update.PullFilter(l => l.LessonContexts, 
            lc => lc.LessonContextId == lessonContextId);
            
        var result = await _lessons.UpdateManyAsync(filter, update);
        
        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"LessonContext with ID {lessonContextId} not found");
        }
    }
    
    public async Task<bool> ExistsAsync(Guid lessonContextId)
    {
        var count = await _lessons.CountDocumentsAsync(
            l => l.LessonContexts.Any(lc => lc.LessonContextId == lessonContextId));
            
        return count > 0;
    }
    
    public async Task<bool> ExistsInLessonAsync(Guid lessonId, Guid lessonContextId)
    {
        var count = await _lessons.CountDocumentsAsync(l => 
            l.LessonId == lessonId && 
            l.LessonContexts.Any(lc => lc.LessonContextId == lessonContextId));
            
        return count > 0;
    }

    public async Task DeactivateAllByIdAsync(Guid lessonContextId)
    {
        var filter = Builders<Lesson>.Filter.ElemMatch(x => x.LessonContexts, lc => lc.LessonContextId == lessonContextId);
        var update = Builders<Lesson>.Update.Set("lesson_contexts.$[elem].is_active", false);
        var arrayFilters = new[] { 
            new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument("elem.lesson_context_id", new BsonBinaryData(lessonContextId, GuidRepresentation.Standard))
            ) 
        };
        var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };
        await _lessons.UpdateManyAsync(filter, update, updateOptions);
    }
}
