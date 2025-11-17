using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Infrastructure.Configuration;
using LessonServiceQuery.Infrastructure.Services;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
namespace LessonServiceQuery.Infrastructure.Persistance.DAOs;
public class CourseDao : ICourseDao
{
    private readonly IMongoCollection<Syllabus> _syllabuses;
    public CourseDao(IMongoDbContext context, IOptions<MongoDbSettings> settings)
    {
        _syllabuses = context.GetCollection<Syllabus>(settings.Value.SyllabusesCollectionName);
    }
    public async Task<Course?> GetByIdAsync(Guid courseId)
    {
        var syllabus = await _syllabuses
            .Find(s => s.IsActive && s.Courses.Any(c => c.CourseId == courseId && c.IsActive))
            .FirstOrDefaultAsync();
        return syllabus?.Courses.FirstOrDefault(c => c.CourseId == courseId && c.IsActive);
    }
    public async Task<List<Course>> GetBySyllabusIdAsync(Guid syllabusId)
    {
        var syllabus = await _syllabuses
            .Find(s => s.SyllabusId == syllabusId && s.IsActive)
            .FirstOrDefaultAsync();
        return syllabus?.Courses.Where(c => c.IsActive).ToList() ?? new List<Course>();
    }
    public async Task<Course> CreateAsync(Guid syllabusId, Course course)
    {
        course.CreatedAt = DateTime.UtcNow;
        course.UpdatedAt = DateTime.UtcNow;
        course.IsActive = true;
        course.SyllabusId = syllabusId;
        
        // Check if syllabus exists
        var existingSyllabus = await _syllabuses
            .Find(s => s.SyllabusId == syllabusId)
            .FirstOrDefaultAsync();
        
        if (existingSyllabus == null)
        {
            // Auto-create Syllabus if not exists (for CQRS event handling)
            var newSyllabus = new Syllabus
            {
                SyllabusId = syllabusId,
                Title = "[Auto-generated] Syllabus for Course",
                Description = "This syllabus was auto-generated when receiving CourseCreated event",
                GradeLevel = "Unknown",
                Subject = "Unknown",
                Version = "1.0",
                Status = "Active",
                CreatedBy = Guid.Empty,
                IsActive = true,
                Courses = new List<Course> { course },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _syllabuses.InsertOneAsync(newSyllabus);
            return course;
        }
        
        // Syllabus exists, push course to it
        var filter = Builders<Syllabus>.Filter.And(
            Builders<Syllabus>.Filter.Eq(s => s.SyllabusId, syllabusId),
            Builders<Syllabus>.Filter.Eq(s => s.IsActive, true)
        );
        var update = Builders<Syllabus>.Update.Push("courses", course);
        
        var result = await _syllabuses.UpdateOneAsync(filter, update);
        
        if (result.MatchedCount == 0)
        {
            throw new InvalidOperationException($"Failed to add Course to Syllabus with ID {syllabusId}");
        }
        
        return course;
    }
    public Task<Course> UpdateAsync(Course course)
    {
        course.UpdatedAt = DateTime.UtcNow;
        return Task.FromResult(course);
    }
    public Task DeleteAsync(Guid courseId)
    {
        return Task.CompletedTask;
    }
    public async Task<bool> ExistsAsync(Guid courseId)
    {
        var count = await _syllabuses
            .CountDocumentsAsync(s => s.Courses.Any(c => c.CourseId == courseId));
        return count > 0;
    }

    public async Task DeactivateAllByIdAsync(Guid courseId)
    {
        var filter = Builders<Syllabus>.Filter.ElemMatch(x => x.Courses, c => c.CourseId == courseId);
        var update = Builders<Syllabus>.Update.Set("courses.$[elem].is_active", false);
        var arrayFilters = new[] { 
            new BsonDocumentArrayFilterDefinition<BsonDocument>(
                new BsonDocument("elem.course_id", new BsonBinaryData(courseId, GuidRepresentation.Standard))
            ) 
        };
        var updateOptions = new UpdateOptions { ArrayFilters = arrayFilters };
        await _syllabuses.UpdateManyAsync(filter, update, updateOptions);
    }
}