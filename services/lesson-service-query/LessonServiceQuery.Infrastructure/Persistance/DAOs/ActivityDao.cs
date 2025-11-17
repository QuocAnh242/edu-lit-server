using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Infrastructure.Configuration;
using LessonServiceQuery.Infrastructure.Services;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LessonServiceQuery.Infrastructure.Persistance.DAOs;

public class ActivityDao : IActivityDao
{
    private readonly IMongoCollection<Lesson> _lessons;
    
    public ActivityDao(IMongoDbContext context, IOptions<MongoDbSettings> settings)
    {
        _lessons = context.GetCollection<Lesson>(settings.Value.LessonsCollectionName);
    }
    
    public async Task<Activity?> GetByIdAsync(Guid activityId)
    {
        var lesson = await _lessons
            .Find(l => l.IsActive && l.LessonContexts.Any(lc => lc.IsActive && lc.Activities.Any(a => a.ActivityId == activityId && a.IsActive)))
            .FirstOrDefaultAsync();
            
        if (lesson == null) return null;
        
        foreach (var lessonContext in lesson.LessonContexts.Where(lc => lc.IsActive))
        {
            var activity = lessonContext.Activities.FirstOrDefault(a => a.ActivityId == activityId && a.IsActive);
            if (activity != null) return activity;
        }
        
        return null;
    }
    
    public async Task<List<Activity>> GetByLessonContextIdAsync(Guid lessonContextId)
    {
        var lesson = await _lessons
            .Find(l => l.IsActive && l.LessonContexts.Any(lc => lc.LessonContextId == lessonContextId && lc.IsActive))
            .FirstOrDefaultAsync();
            
        var lessonContext = lesson?.LessonContexts.FirstOrDefault(lc => lc.LessonContextId == lessonContextId && lc.IsActive);
        
        return lessonContext?.Activities.Where(a => a.IsActive).ToList() ?? new List<Activity>();
    }
    
    public async Task<Activity> CreateAsync(Guid lessonContextId, Activity activity)
    {
        activity.CreatedAt = DateTime.UtcNow;
        activity.UpdatedAt = DateTime.UtcNow;
        
        // Check if lesson context exists
        var lessonContextExists = await _lessons.CountDocumentsAsync(
            l => l.LessonContexts.Any(lc => lc.LessonContextId == lessonContextId)) > 0;
            
        if (!lessonContextExists)
        {
            throw new InvalidOperationException($"LessonContext with ID {lessonContextId} not found");
        }
        
        // Check if activity already exists in the lesson context
        var existsInLessonContext = await ExistsInLessonContextAsync(lessonContextId, activity.ActivityId);
        if (existsInLessonContext)
        {
            throw new InvalidOperationException($"Activity with ID {activity.ActivityId} already exists in lesson context {lessonContextId}");
        }
        
        // Add the activity to the specific lesson context's activities array
        var filter = Builders<Lesson>.Filter.ElemMatch(l => l.LessonContexts, 
            lc => lc.LessonContextId == lessonContextId);
            
        var update = Builders<Lesson>.Update.Push("lesson_contexts.$.activities", activity);
        
        var result = await _lessons.UpdateOneAsync(filter, update);
        
        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Failed to add Activity to LessonContext with ID {lessonContextId}");
        }
        
        return activity;
    }
    
    public async Task<Activity> UpdateAsync(Activity activity)
    {
        activity.UpdatedAt = DateTime.UtcNow;
        
        // Find lesson containing the activity
        var lesson = await _lessons
            .Find(l => l.LessonContexts.Any(lc => lc.Activities.Any(a => a.ActivityId == activity.ActivityId)))
            .FirstOrDefaultAsync();
            
        if (lesson == null)
        {
            throw new InvalidOperationException($"Activity with ID {activity.ActivityId} not found");
        }
        
        // Find the exact position and update
        for (int i = 0; i < lesson.LessonContexts.Count; i++)
        {
            var lessonContext = lesson.LessonContexts[i];
            for (int j = 0; j < lessonContext.Activities.Count; j++)
            {
                if (lessonContext.Activities[j].ActivityId == activity.ActivityId)
                {
                    var filter = Builders<Lesson>.Filter.And(
                        Builders<Lesson>.Filter.Eq(l => l.LessonId, lesson.LessonId),
                        Builders<Lesson>.Filter.ElemMatch(l => l.LessonContexts, 
                            lc => lc.LessonContextId == lessonContext.LessonContextId)
                    );
                    
                    var update = Builders<Lesson>.Update.Set($"lesson_contexts.{i}.activities.{j}", activity);
                    
                    await _lessons.UpdateOneAsync(filter, update);
                    return activity;
                }
            }
        }
        
        throw new InvalidOperationException($"Activity with ID {activity.ActivityId} not found for update");
    }
    
    public async Task DeleteAsync(Guid activityId)
    {
        var filter = Builders<Lesson>.Filter.ElemMatch(l => l.LessonContexts,
            lc => lc.Activities.Any(a => a.ActivityId == activityId));
            
        var update = Builders<Lesson>.Update.PullFilter("lesson_contexts.$[].activities", 
            Builders<Activity>.Filter.Eq(a => a.ActivityId, activityId));
            
        var result = await _lessons.UpdateManyAsync(filter, update);
        
        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Activity with ID {activityId} not found");
        }
    }
    
    public async Task<bool> ExistsAsync(Guid activityId)
    {
        var count = await _lessons.CountDocumentsAsync(
            l => l.LessonContexts.Any(lc => lc.Activities.Any(a => a.ActivityId == activityId)));
            
        return count > 0;
    }
    
    public async Task<bool> ExistsInLessonContextAsync(Guid lessonContextId, Guid activityId)
    {
        var count = await _lessons.CountDocumentsAsync(l => 
            l.LessonContexts.Any(lc => 
                lc.LessonContextId == lessonContextId && 
                lc.Activities.Any(a => a.ActivityId == activityId)));
                
        return count > 0;
    }

    public async Task DeactivateAllByIdAsync(Guid activityId)
    {
        var filter = Builders<Lesson>.Filter.ElemMatch(x => x.LessonContexts, 
            lc => lc.Activities.Any(a => a.ActivityId == activityId));
        var update = Builders<Lesson>.Update.Set("lesson_contexts.$[].activities.$[elem].is_active", false);
        var arrayFilters = new[] { 
            new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument("elem.activity_id", new BsonBinaryData(activityId, GuidRepresentation.Standard))
            ) 
        };
        var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };
        await _lessons.UpdateManyAsync(filter, update, updateOptions);
    }
}
