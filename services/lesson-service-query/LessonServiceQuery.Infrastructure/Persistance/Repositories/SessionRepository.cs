using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Domain.Repositories;
namespace LessonServiceQuery.Infrastructure.Persistance.Repositories;
public class SessionRepository : ISessionRepository
{
    private readonly ISessionDao _sessionDao;
    public SessionRepository(ISessionDao sessionDao)
    {
        _sessionDao = sessionDao;
    }
    public async Task<Session?> GetByIdAsync(Guid sessionId)
    {
        return await _sessionDao.GetByIdAsync(sessionId);
    }
    public async Task<List<Session>> GetByCourseIdAsync(Guid courseId)
    {
        return await _sessionDao.GetByCourseIdAsync(courseId);
    }
    public async Task<List<Session>> GetByLessonIdAsync(Guid lessonId)
    {
        return await _sessionDao.GetByLessonIdAsync(lessonId);
    }
    public Task<Session> CreateAsync(Session session)
    {
        // Note: Repository pattern doesn't have CourseId context
        // This method should not be used directly. Use DAO through Event Handlers instead.
        throw new NotSupportedException("CreateAsync at Repository level is not supported. Use SessionDao.CreateAsync(courseId, session) instead.");
    }
    public async Task<Session> UpdateAsync(Session session)
    {
        return await _sessionDao.UpdateAsync(session);
    }
    public async Task DeleteAsync(Guid sessionId)
    {
        await _sessionDao.DeleteAsync(sessionId);
    }
    public async Task<bool> ExistsAsync(Guid sessionId)
    {
        return await _sessionDao.ExistsAsync(sessionId);
    }
}