using LessonServiceQuery.Domain.Entities;

namespace LessonServiceQuery.Domain.IDAOs;

public interface ILessonContextDao
{
    Task<LessonContext?> GetByIdAsync(Guid lessonContextId);
    Task<List<LessonContext>> GetByLessonIdAsync(Guid lessonId);
    Task<LessonContext> CreateAsync(Guid lessonId, LessonContext lessonContext);
    Task<LessonContext> UpdateAsync(LessonContext lessonContext);
    Task DeleteAsync(Guid lessonContextId);
    Task<bool> ExistsAsync(Guid lessonContextId);
    Task<bool> ExistsInLessonAsync(Guid lessonId, Guid lessonContextId);
    Task DeactivateAllByIdAsync(Guid lessonContextId);
}
