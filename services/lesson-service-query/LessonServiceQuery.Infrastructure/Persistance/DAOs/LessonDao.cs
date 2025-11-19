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
    private readonly ILessonContextDao _lessonContextDao;
    private readonly IActivityDao _activityDao;
    
    public LessonDao(
        IMongoDbContext context, 
        IOptions<MongoDbSettings> settings,
        ILessonContextDao lessonContextDao,
        IActivityDao activityDao)
    {
        _lessons = context.GetCollection<Lesson>(settings.Value.LessonsCollectionName);
        _lessonContextDao = lessonContextDao;
        _activityDao = activityDao;
    }
    
    public async Task<Lesson?> GetByIdAsync(Guid lessonId)
    {
        var lesson = await _lessons.Find(x => x.LessonId == lessonId && x.IsActive).FirstOrDefaultAsync();
        
        if (lesson != null)
        {
            // Populate LessonContexts and Activities from separate collections
            lesson.LessonContexts = await _lessonContextDao.GetByLessonIdAsync(lessonId);
            lesson.Activities = await _activityDao.GetByLessonIdAsync(lessonId);
        }
        
        return lesson;
    }
    
    public async Task<List<Lesson>> GetAllAsync()
    {
        var lessons = await _lessons.Find(x => x.IsActive).ToListAsync();
        
        // Populate each lesson with its contexts and activities
        foreach (var lesson in lessons)
        {
            lesson.LessonContexts = await _lessonContextDao.GetByLessonIdAsync(lesson.LessonId);
            lesson.Activities = await _activityDao.GetByLessonIdAsync(lesson.LessonId);
        }
        
        return lessons;
    }
    
    public async Task<List<Lesson>> GetBySessionIdAsync(Guid sessionId)
    {
        var lessons = await _lessons.Find(x => x.SessionId == sessionId && x.IsActive)
            .SortBy(x => x.Position)
            .ToListAsync();
        
        // Populate each lesson with its contexts and activities
        foreach (var lesson in lessons)
        {
            lesson.LessonContexts = await _lessonContextDao.GetByLessonIdAsync(lesson.LessonId);
            lesson.Activities = await _activityDao.GetByLessonIdAsync(lesson.LessonId);
        }
        
        return lessons;
    }
    
    public async Task<List<Lesson>> GetByTeacherIdAsync(Guid teacherId)
    {
        var lessons = await _lessons.Find(x => x.IsActive).ToListAsync();
        
        // Populate each lesson with its contexts and activities
        foreach (var lesson in lessons)
        {
            lesson.LessonContexts = await _lessonContextDao.GetByLessonIdAsync(lesson.LessonId);
            lesson.Activities = await _activityDao.GetByLessonIdAsync(lesson.LessonId);
        }
        
        return lessons;
    }
    
    public async Task<List<Lesson>> GetBySubjectAsync(string subject)
    {
        var lessons = await _lessons.Find(x => x.IsActive).ToListAsync();
        
        // Populate each lesson with its contexts and activities
        foreach (var lesson in lessons)
        {
            lesson.LessonContexts = await _lessonContextDao.GetByLessonIdAsync(lesson.LessonId);
            lesson.Activities = await _activityDao.GetByLessonIdAsync(lesson.LessonId);
        }
        
        return lessons;
    }
    
    public async Task<List<Lesson>> GetByGradeLevelAsync(string gradeLevel)
    {
        var lessons = await _lessons.Find(x => x.IsActive).ToListAsync();
        
        // Populate each lesson with its contexts and activities
        foreach (var lesson in lessons)
        {
            lesson.LessonContexts = await _lessonContextDao.GetByLessonIdAsync(lesson.LessonId);
            lesson.Activities = await _activityDao.GetByLessonIdAsync(lesson.LessonId);
        }
        
        return lessons;
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
        // Soft delete: set all versions of this lesson to IsActive = false
        await DeactivateAllByIdAsync(lessonId);
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