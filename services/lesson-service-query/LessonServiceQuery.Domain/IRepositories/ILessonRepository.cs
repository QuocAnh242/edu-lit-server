using LessonServiceQuery.Domain.Entities;

namespace LessonServiceQuery.Domain.Repositories;

public interface ILessonRepository
{
    Task<Lesson?> GetByIdAsync(Guid lessonId);
    Task<List<Lesson>> GetAllAsync();
    Task<List<Lesson>> GetByTeacherIdAsync(Guid teacherId);
    Task<List<Lesson>> GetBySubjectAsync(string subject);
    Task<List<Lesson>> GetByGradeLevelAsync(string gradeLevel);
    Task<Lesson> CreateAsync(Lesson lesson);
    Task<Lesson> UpdateAsync(Lesson lesson);
    Task DeleteAsync(Guid lessonId);
    Task<bool> ExistsAsync(Guid lessonId);
}
