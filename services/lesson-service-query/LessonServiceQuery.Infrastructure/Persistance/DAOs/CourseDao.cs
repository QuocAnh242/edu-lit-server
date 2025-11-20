using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Infrastructure.Configuration;
using LessonServiceQuery.Infrastructure.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LessonServiceQuery.Infrastructure.Persistance.DAOs;

public class CourseDao : ICourseDao
{
    private readonly IMongoCollection<Course> _courses;
    
    public CourseDao(IMongoDbContext context, IOptions<MongoDbSettings> settings)
    {
        _courses = context.GetCollection<Course>(settings.Value.CoursesCollectionName);
    }
    
    public async Task<Course?> GetByIdAsync(Guid courseId)
    {
        return await _courses.Find(x => x.CourseId == courseId && x.IsActive).FirstOrDefaultAsync();
    }
    
    public async Task<List<Course>> GetBySyllabusIdAsync(Guid syllabusId)
    {
        return await _courses.Find(x => x.SyllabusId == syllabusId && x.IsActive).ToListAsync();
    }
    
    public async Task<List<Course>> GetAllAsync()
    {
        return await _courses.Find(x => x.IsActive).ToListAsync();
    }
    
    public async Task<Course> CreateAsync(Guid syllabusId, Course course)
    {
        course.SyllabusId = syllabusId;
        course.CreatedAt = DateTime.UtcNow;
        course.UpdatedAt = DateTime.UtcNow;
        course.IsActive = true;
        
        await _courses.InsertOneAsync(course);
        return course;
    }
    
    public async Task<Course> UpdateAsync(Course course)
    {
        course.UpdatedAt = DateTime.UtcNow;
        await _courses.ReplaceOneAsync(x => x.CourseId == course.CourseId, course);
        return course;
    }
    
    public async Task DeleteAsync(Guid courseId)
    {
        // Soft delete: set all versions of this course to IsActive = false
        await DeactivateAllByIdAsync(courseId);
    }
    
    public async Task<bool> ExistsAsync(Guid courseId)
    {
        return await _courses.CountDocumentsAsync(x => x.CourseId == courseId) > 0;
    }

    public async Task DeactivateAllByIdAsync(Guid courseId)
    {
        var filter = Builders<Course>.Filter.Eq(x => x.CourseId, courseId);
        var update = Builders<Course>.Update.Set(x => x.IsActive, false);
        await _courses.UpdateManyAsync(filter, update);
    }
}