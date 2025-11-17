using LessonServiceQuery.Domain.Entities;
namespace LessonServiceQuery.Domain.IDAOs;
public interface ISessionDao
{
    Task<Session?> GetByIdAsync(Guid sessionId);
    Task<List<Session>> GetByCourseIdAsync(Guid courseId);
    Task<List<Session>> GetByLessonIdAsync(Guid lessonId);
    Task<Session> CreateAsync(Guid courseId, Session session);
    Task<Session> UpdateAsync(Session session);
    Task DeleteAsync(Guid sessionId);
    Task<bool> ExistsAsync(Guid sessionId);
    Task DeactivateAllByIdAsync(Guid sessionId);
}
