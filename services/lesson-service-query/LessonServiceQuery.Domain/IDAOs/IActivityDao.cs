using LessonServiceQuery.Domain.Entities;

namespace LessonServiceQuery.Domain.IDAOs;

public interface IActivityDao
{
    Task<Activity?> GetByIdAsync(Guid activityId);
    Task<List<Activity>> GetByLessonContextIdAsync(Guid lessonContextId);
    Task<List<Activity>> GetByLessonIdAsync(Guid lessonId);
    Task<Activity> CreateAsync(Guid lessonContextId, Activity activity);
    Task<Activity> UpdateAsync(Activity activity);
    Task DeleteAsync(Guid activityId);
    Task<bool> ExistsAsync(Guid activityId);
    Task<bool> ExistsInLessonContextAsync(Guid lessonContextId, Guid activityId);
    Task DeactivateAllByIdAsync(Guid activityId);
}
