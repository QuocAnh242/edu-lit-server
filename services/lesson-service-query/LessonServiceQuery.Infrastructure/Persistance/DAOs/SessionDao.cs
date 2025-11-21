using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Infrastructure.Configuration;
using LessonServiceQuery.Infrastructure.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LessonServiceQuery.Infrastructure.Persistance.DAOs;

public class SessionDao : ISessionDao
{
    private readonly IMongoCollection<Session> _sessions;
    
    public SessionDao(IMongoDbContext context, IOptions<MongoDbSettings> settings)
    {
        _sessions = context.GetCollection<Session>(settings.Value.SessionsCollectionName);
    }
    
    public async Task<Session?> GetByIdAsync(Guid sessionId)
    {
        return await _sessions.Find(x => x.SessionId == sessionId && x.IsActive).FirstOrDefaultAsync();
    }
    
    public async Task<List<Session>> GetByCourseIdAsync(Guid courseId)
    {
        return await _sessions.Find(x => x.CourseId == courseId && x.IsActive)
            .SortBy(x => x.Position)
            .ToListAsync();
    }
    
    public async Task<List<Session>> GetByLessonIdAsync(Guid lessonId)
    {
        // Note: This method may need to be updated based on your data model
        // Since Session doesn't have LessonId directly, you might need to query Lesson collection
        return new List<Session>();
    }
    
    public async Task<Session> CreateAsync(Guid courseId, Session session)
    {
        session.CourseId = courseId;
        session.CreatedAt = DateTime.UtcNow;
        session.UpdatedAt = DateTime.UtcNow;
        session.IsActive = true;
        
        await _sessions.InsertOneAsync(session);
        return session;
    }
    
    public async Task<Session> UpdateAsync(Session session)
    {
        session.UpdatedAt = DateTime.UtcNow;
        await _sessions.ReplaceOneAsync(x => x.SessionId == session.SessionId, session);
        return session;
    }
    
    public async Task DeleteAsync(Guid sessionId)
    {
        // Soft delete: set all versions of this session to IsActive = false
        await DeactivateAllByIdAsync(sessionId);
    }
    
    public async Task<bool> ExistsAsync(Guid sessionId)
    {
        return await _sessions.CountDocumentsAsync(x => x.SessionId == sessionId) > 0;
    }

    public async Task DeactivateAllByIdAsync(Guid sessionId)
    {
        var filter = Builders<Session>.Filter.Eq(x => x.SessionId, sessionId);
        var update = Builders<Session>.Update.Set(x => x.IsActive, false);
        await _sessions.UpdateManyAsync(filter, update);
    }
}