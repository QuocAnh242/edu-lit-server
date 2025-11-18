using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Infrastructure.Configuration;
using LessonServiceQuery.Infrastructure.Services;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
namespace LessonServiceQuery.Infrastructure.Persistance.DAOs;
public class SessionDao : ISessionDao
{
    private readonly IMongoCollection<Syllabus> _syllabuses;
    public SessionDao(IMongoDbContext context, IOptions<MongoDbSettings> settings)
    {
        _syllabuses = context.GetCollection<Syllabus>(settings.Value.SyllabusesCollectionName);
    }
    public async Task<Session?> GetByIdAsync(Guid sessionId)
    {
        var syllabus = await _syllabuses
            .Find(s => s.IsActive && s.Courses.Any(c => c.IsActive && c.Sessions.Any(sess => sess.SessionId == sessionId && sess.IsActive)))
            .FirstOrDefaultAsync();
        if (syllabus == null) return null;
        foreach (var course in syllabus.Courses.Where(c => c.IsActive))
        {
            var session = course.Sessions.FirstOrDefault(s => s.SessionId == sessionId && s.IsActive);
            if (session != null) return session;
        }
        return null;
    }
    public async Task<List<Session>> GetByCourseIdAsync(Guid courseId)
    {
        var syllabus = await _syllabuses
            .Find(s => s.IsActive && s.Courses.Any(c => c.CourseId == courseId && c.IsActive))
            .FirstOrDefaultAsync();
        if (syllabus == null) return new List<Session>();
        var course = syllabus.Courses.FirstOrDefault(c => c.CourseId == courseId && c.IsActive);
        return course?.Sessions.Where(s => s.IsActive).ToList() ?? new List<Session>();
    }
    public async Task<List<Session>> GetByLessonIdAsync(Guid lessonId)
    {
        var syllabuses = await _syllabuses
            .Find(s => s.IsActive && s.Courses.Any(c => c.IsActive && c.Sessions.Any(sess => sess.LessonId == lessonId && sess.IsActive)))
            .ToListAsync();
        var sessions = new List<Session>();
        foreach (var syllabus in syllabuses)
        {
            foreach (var course in syllabus.Courses.Where(c => c.IsActive))
            {
                sessions.AddRange(course.Sessions.Where(s => s.LessonId == lessonId && s.IsActive));
            }
        }
        return sessions;
    }
    public async Task<Session> CreateAsync(Guid courseId, Session session)
    {
        session.CreatedAt = DateTime.UtcNow;
        session.UpdatedAt = DateTime.UtcNow;
        session.IsActive = true;
        
        // Check if course exists in any syllabus
        var syllabusWithCourse = await _syllabuses
            .Find(s => s.IsActive && s.Courses.Any(c => c.CourseId == courseId && c.IsActive))
            .FirstOrDefaultAsync();
            
        if (syllabusWithCourse == null)
        {
            throw new InvalidOperationException($"Course with ID {courseId} not found or not active");
        }
        
        // Log for debugging
        Console.WriteLine($"Found Syllabus: {syllabusWithCourse.SyllabusId}, CourseId to add: {courseId}");
        
        // Add the session to the course's sessions array using positional operator
        var filter = Builders<Syllabus>.Filter.And(
            Builders<Syllabus>.Filter.Eq(s => s.SyllabusId, syllabusWithCourse.SyllabusId),
            Builders<Syllabus>.Filter.ElemMatch(s => s.Courses, c => c.CourseId == courseId)
        );
        var update = Builders<Syllabus>.Update.Push("courses.$.sessions", session);
        
        var result = await _syllabuses.UpdateOneAsync(filter, update);
        
        Console.WriteLine($"Update result - MatchedCount: {result.MatchedCount}, ModifiedCount: {result.ModifiedCount}");
        
        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Failed to add Session to Course with ID {courseId}. Syllabus: {syllabusWithCourse.SyllabusId}");
        }
        
        return session;
    }
    public Task<Session> UpdateAsync(Session session)
    {
        session.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(session);
    }
    public Task DeleteAsync(Guid sessionId)
    {
        return Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid sessionId)
    {
        var count = await _syllabuses
            .CountDocumentsAsync(s => s.Courses.Any(c => c.Sessions.Any(sess => sess.SessionId == sessionId)));
        return count > 0;
    }

    public async Task DeactivateAllByIdAsync(Guid sessionId)
    {
        var filter = Builders<Syllabus>.Filter.ElemMatch(x => x.Courses, c => c.Sessions.Any(s => s.SessionId == sessionId));
        var update = Builders<Syllabus>.Update.Set("courses.$[].sessions.$[elem].is_active", false);
        var arrayFilters = new[] { 
            new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument("elem.session_id", new BsonBinaryData(sessionId, GuidRepresentation.Standard))
            ) 
        };
        var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };
        await _syllabuses.UpdateManyAsync(filter, update, updateOptions);
    }
}