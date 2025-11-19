using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Domain.Repositories;

namespace LessonServiceQuery.Infrastructure.Persistance.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly ICourseDao _courseDao;
    private readonly ISessionDao _sessionDao;

    public CourseRepository(ICourseDao courseDao, ISessionDao sessionDao)
    {
        _courseDao = courseDao;
        _sessionDao = sessionDao;
    }

    public async Task<Course?> GetByIdAsync(Guid courseId)
    {
        var course = await _courseDao.GetByIdAsync(courseId);
        if (course != null)
        {
            // Populate sessions from separate collection
            var sessions = await _sessionDao.GetByCourseIdAsync(courseId);
            course.Sessions = sessions;
        }
        return course;
    }

    public async Task<List<Course>> GetBySyllabusIdAsync(Guid syllabusId)
    {
        var courses = await _courseDao.GetBySyllabusIdAsync(syllabusId);
        
        // Populate sessions for each course
        foreach (var course in courses)
        {
            var sessions = await _sessionDao.GetByCourseIdAsync(course.CourseId);
            course.Sessions = sessions;
        }
        
        return courses;
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