using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Infrastructure.Configuration;
using LessonServiceQuery.Infrastructure.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
namespace LessonServiceQuery.Infrastructure.Persistance.DAOs;
public class SyllabusDao : ISyllabusDao
{
    private readonly IMongoCollection<Syllabus> _syllabuses;
    public SyllabusDao(IMongoDbContext context, IOptions<MongoDbSettings> settings)
    {
        _syllabuses = context.GetCollection<Syllabus>(settings.Value.SyllabusesCollectionName);
    }
    public async Task<Syllabus?> GetByIdAsync(Guid syllabusId)
    {
        return await _syllabuses.Find(x => x.SyllabusId == syllabusId && x.IsActive).FirstOrDefaultAsync();
    }
    public async Task<List<Syllabus>> GetAllAsync()
    {
        return await _syllabuses.Find(x => x.IsActive).ToListAsync();
    }
    public async Task<List<Syllabus>> GetByCreatorIdAsync(Guid creatorId)
    {
        return await _syllabuses.Find(x => x.CreatedBy == creatorId && x.IsActive).ToListAsync();
    }
    public async Task<List<Syllabus>> GetBySubjectAsync(string subject)
    {
        return await _syllabuses.Find(x => x.Subject == subject && x.IsActive).ToListAsync();
    }
    public async Task<List<Syllabus>> GetByGradeLevelAsync(string gradeLevel)
    {
        return await _syllabuses.Find(x => x.GradeLevel == gradeLevel && x.IsActive).ToListAsync();
    }
    public async Task<List<Syllabus>> GetByStatusAsync(string status)
    {
        return await _syllabuses.Find(x => x.Status == status && x.IsActive).ToListAsync();
    }
    public async Task<Syllabus> CreateAsync(Syllabus syllabus)
    {
        syllabus.CreatedAt = DateTime.UtcNow;
        syllabus.UpdatedAt = DateTime.UtcNow;
        await _syllabuses.InsertOneAsync(syllabus);
        return syllabus;
    }
    public async Task<Syllabus> UpdateAsync(Syllabus syllabus)
    {
        syllabus.UpdatedAt = DateTime.UtcNow;
        await _syllabuses.ReplaceOneAsync(x => x.SyllabusId == syllabus.SyllabusId, syllabus);
        return syllabus;
    }
    public async Task DeleteAsync(Guid syllabusId)
    {
        await _syllabuses.DeleteOneAsync(x => x.SyllabusId == syllabusId);
    }
    public async Task<bool> ExistsAsync(Guid syllabusId)
    {
        return await _syllabuses.CountDocumentsAsync(x => x.SyllabusId == syllabusId) > 0;
    }

    public async Task DeactivateAllByIdAsync(Guid syllabusId)
    {
        var filter = Builders<Syllabus>.Filter.Eq(x => x.SyllabusId, syllabusId);
        var update = Builders<Syllabus>.Update.Set(x => x.IsActive, false);
        await _syllabuses.UpdateManyAsync(filter, update);
    }
}