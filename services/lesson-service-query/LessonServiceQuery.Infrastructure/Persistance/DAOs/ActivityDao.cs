using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Infrastructure.Configuration;
using LessonServiceQuery.Infrastructure.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LessonServiceQuery.Infrastructure.Persistance.DAOs;

public class ActivityDao : IActivityDao
{
    private readonly IMongoCollection<Activity> _activities;
    
    public ActivityDao(IMongoDbContext context, IOptions<MongoDbSettings> settings)
    {
        _activities = context.GetCollection<Activity>(settings.Value.ActivitiesCollectionName);
    }
    
    public async Task<Activity?> GetByIdAsync(Guid activityId)
    {
        return await _activities.Find(x => x.ActivityId == activityId && x.IsActive)
            .FirstOrDefaultAsync();
    }
    
    public async Task<List<Activity>> GetByLessonContextIdAsync(Guid lessonContextId)
    {
        // This method is deprecated - activities are now at lesson level
        //too old
        return new List<Activity>();
    }
    
    public async Task<List<Activity>> GetByLessonIdAsync(Guid lessonId)
    {
        return await _activities.Find(x => x.LessonId == lessonId && x.IsActive)
            .SortBy(x => x.Position)
            .ToListAsync();
    }
    
    public async Task<Activity> CreateAsync(Guid lessonId, Activity activity)
    {
        activity.LessonId = lessonId;
        activity.CreatedAt = DateTime.UtcNow;
        activity.UpdatedAt = DateTime.UtcNow;
        activity.IsActive = true;
        
        await _activities.InsertOneAsync(activity);
        return activity;
    }
    
    public async Task<Activity> UpdateAsync(Activity activity)
    {
        activity.UpdatedAt = DateTime.UtcNow;
        await _activities.ReplaceOneAsync(
            x => x.ActivityId == activity.ActivityId && x.IsActive,
            activity);
        return activity;
    }
    
    public async Task DeleteAsync(Guid activityId)
    {
        // Soft delete: set all versions to IsActive = false
        await DeactivateAllByIdAsync(activityId);
    }
    
    public async Task<bool> ExistsAsync(Guid activityId)
    {
        return await _activities.CountDocumentsAsync(x => x.ActivityId == activityId) > 0;
    }
    
    public async Task<bool> ExistsInLessonContextAsync(Guid lessonContextId, Guid activityId)
    {
        // Deprecated - activities are now at lesson level
        return false;
    }

    public async Task DeactivateAllByIdAsync(Guid activityId)
    {
        var filter = Builders<Activity>.Filter.Eq(x => x.ActivityId, activityId);
        var update = Builders<Activity>.Update.Set(x => x.IsActive, false);
        await _activities.UpdateManyAsync(filter, update);
    }
}
