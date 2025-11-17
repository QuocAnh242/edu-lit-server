using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Infrastructure.Configuration;
using LessonServiceQuery.Infrastructure.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LessonServiceQuery.Infrastructure.Persistance.DAOs;

public class LessonDao : ILessonDao
{
    private readonly IMongoCollection<Lesson> _lessons;
    
    public LessonDao(IMongoDbContext context, IOptions<MongoDbSettings> settings)
    {
        _lessons = context.GetCollection<Lesson>(settings.Value.LessonsCollectionName);
    }
    
    public async Task<Lesson?> GetByIdAsync(Guid lessonId)
    {
        return await _lessons.Find(x => x.LessonId == lessonId && x.IsActive).FirstOrDefaultAsync();
    }
    
    public async Task<List<Lesson>> GetAllAsync()
    {
        return await _lessons.Find(x => x.IsActive).ToListAsync();
    }
    
    public async Task<List<Lesson>> GetByTeacherIdAsync(Guid teacherId)
    {
        return await _lessons.Find(x => x.TeacherId == teacherId && x.IsActive).ToListAsync();
    }
    
    public async Task<List<Lesson>> GetBySubjectAsync(string subject)
    {
        return await _lessons.Find(x => x.Subject == subject && x.IsActive).ToListAsync();
    }
    
    public async Task<List<Lesson>> GetByGradeLevelAsync(string gradeLevel)
    {
        return await _lessons.Find(x => x.GradeLevel == gradeLevel && x.IsActive).ToListAsync();
    }
    
    public async Task<Lesson> CreateAsync(Lesson lesson)
    {
        lesson.CreatedAt = DateTime.UtcNow;
        lesson.UpdatedAt = DateTime.UtcNow;
        await _lessons.InsertOneAsync(lesson);
        return lesson;
    }
    
    public async Task<Lesson> UpdateAsync(Lesson lesson)
    {
        lesson.UpdatedAt = DateTime.UtcNow;
        await _lessons.ReplaceOneAsync(x => x.LessonId == lesson.LessonId, lesson);
        return lesson;
    }
    
    public async Task DeleteAsync(Guid lessonId)
    {
        await _lessons.DeleteOneAsync(x => x.LessonId == lessonId);
    }
    
    public async Task<bool> ExistsAsync(Guid lessonId)
    {
        return await _lessons.CountDocumentsAsync(x => x.LessonId == lessonId) > 0;
    }

    public async Task DeactivateAllByIdAsync(Guid lessonId)
    {
        var filter = Builders<Lesson>.Filter.Eq(x => x.LessonId, lessonId);
        var update = Builders<Lesson>.Update.Set(x => x.IsActive, false);
        await _lessons.UpdateManyAsync(filter, update);
    }
}