using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Domain.Repositories;
namespace LessonServiceQuery.Infrastructure.Persistance.Repositories;
public class SyllabusRepository : ISyllabusRepository
{
    private readonly ISyllabusDao _syllabusDao;
    public SyllabusRepository(ISyllabusDao syllabusDao)
    {
        _syllabusDao = syllabusDao;
    }
    public async Task<Syllabus?> GetByIdAsync(Guid syllabusId)
    {
        return await _syllabusDao.GetByIdAsync(syllabusId);
    }
    public async Task<List<Syllabus>> GetAllAsync()
    {
        return await _syllabusDao.GetAllAsync();
    }
    public async Task<List<Syllabus>> GetByCreatorIdAsync(Guid creatorId)
    {
        return await _syllabusDao.GetByCreatorIdAsync(creatorId);
    }
    public async Task<List<Syllabus>> GetBySubjectAsync(string subject)
    {
        return await _syllabusDao.GetBySubjectAsync(subject);
    }
    public async Task<List<Syllabus>> GetByGradeLevelAsync(string gradeLevel)
    {
        return await _syllabusDao.GetByGradeLevelAsync(gradeLevel);
    }
    public async Task<List<Syllabus>> GetByStatusAsync(string status)
    {
        return await _syllabusDao.GetByStatusAsync(status);
    }
    public async Task<Syllabus> CreateAsync(Syllabus syllabus)
    {
        return await _syllabusDao.CreateAsync(syllabus);
    }
    public async Task<Syllabus> UpdateAsync(Syllabus syllabus)
    {
        return await _syllabusDao.UpdateAsync(syllabus);
    }
    public async Task DeleteAsync(Guid syllabusId)
    {
        await _syllabusDao.DeleteAsync(syllabusId);
    }
    public async Task<bool> ExistsAsync(Guid syllabusId)
    {
        return await _syllabusDao.ExistsAsync(syllabusId);
    }
}