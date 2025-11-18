using LessonServiceQuery.Domain.Entities;
namespace LessonServiceQuery.Domain.Repositories;
public interface ISessionRepository
{
    Task<Session?> GetByIdAsync(Guid sessionId);
    Task<List<Session>> GetByCourseIdAsync(Guid courseId);
    Task<List<Session>> GetByLessonIdAsync(Guid lessonId);
    Task<Session> CreateAsync(Session session);
    Task<Session> UpdateAsync(Session session);
    Task DeleteAsync(Guid sessionId);
    Task<bool> ExistsAsync(Guid sessionId);
}
