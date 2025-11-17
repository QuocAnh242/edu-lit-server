using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Domain.Repositories;

namespace LessonServiceQuery.Infrastructure.Persistance.Repositories;

public class LessonRepository : ILessonRepository
{
    private readonly ILessonDao _lessonDao;
    
    public LessonRepository(ILessonDao lessonDao)
    {
        _lessonDao = lessonDao;
    }
    
    public async Task<Lesson?> GetByIdAsync(Guid lessonId)
    {
        return await _lessonDao.GetByIdAsync(lessonId);
    }
    
    public async Task<List<Lesson>> GetAllAsync()
    {
        return await _lessonDao.GetAllAsync();
    }
    
    public async Task<List<Lesson>> GetByTeacherIdAsync(Guid teacherId)
    {
        return await _lessonDao.GetByTeacherIdAsync(teacherId);
    }
    
    public async Task<List<Lesson>> GetBySubjectAsync(string subject)
    {
        return await _lessonDao.GetBySubjectAsync(subject);
    }
    
    public async Task<List<Lesson>> GetByGradeLevelAsync(string gradeLevel)
    {
        return await _lessonDao.GetByGradeLevelAsync(gradeLevel);
    }
    
    public async Task<Lesson> CreateAsync(Lesson lesson)
    {
        return await _lessonDao.CreateAsync(lesson);
    }
    
    public async Task<Lesson> UpdateAsync(Lesson lesson)
    {
        return await _lessonDao.UpdateAsync(lesson);
    }
    
    public async Task DeleteAsync(Guid lessonId)
    {
        await _lessonDao.DeleteAsync(lessonId);
    }
    
    public async Task<bool> ExistsAsync(Guid lessonId)
    {
        return await _lessonDao.ExistsAsync(lessonId);
    }
}
