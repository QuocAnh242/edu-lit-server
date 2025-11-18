using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Domain.Repositories;
namespace LessonServiceQuery.Infrastructure.Persistance.Repositories;
public class CourseRepository : ICourseRepository
{
    private readonly ICourseDao _courseDao;
    public CourseRepository(ICourseDao courseDao)
    {
        _courseDao = courseDao;
    }
    public async Task<Course?> GetByIdAsync(Guid courseId)
    {
        return await _courseDao.GetByIdAsync(courseId);
    }
    public async Task<List<Course>> GetBySyllabusIdAsync(Guid syllabusId)
    {
        return await _courseDao.GetBySyllabusIdAsync(syllabusId);
    }
    public Task<Course> CreateAsync(Course course)
    {
        // Note: Repository pattern doesn't have SyllabusId context
        // This method should not be used directly. Use DAO through Event Handlers instead.
        throw new NotSupportedException("CreateAsync at Repository level is not supported. Use CourseDao.CreateAsync(syllabusId, course) instead.");
    }
    public async Task<Course> UpdateAsync(Course course)
    {
        return await _courseDao.UpdateAsync(course);
    }
    public async Task DeleteAsync(Guid courseId)
    {
        await _courseDao.DeleteAsync(courseId);
    }
    public async Task<bool> ExistsAsync(Guid courseId)
    {
        return await _courseDao.ExistsAsync(courseId);
    }
}