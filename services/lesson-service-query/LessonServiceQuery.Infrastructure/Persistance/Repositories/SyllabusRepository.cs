using LessonServiceQuery.Domain.Entities;
using LessonServiceQuery.Domain.IDAOs;
using LessonServiceQuery.Domain.Repositories;

namespace LessonServiceQuery.Infrastructure.Persistance.Repositories;

public class SyllabusRepository : ISyllabusRepository
{
    private readonly ISyllabusDao _syllabusDao;
    private readonly ICourseDao _courseDao;
    private readonly ISessionDao _sessionDao;

    public SyllabusRepository(ISyllabusDao syllabusDao, ICourseDao courseDao, ISessionDao sessionDao)
    {
        _syllabusDao = syllabusDao;
        _courseDao = courseDao;
        _sessionDao = sessionDao;
    }

    public async Task<Syllabus?> GetByIdAsync(Guid syllabusId)
    {
        var syllabus = await _syllabusDao.GetByIdAsync(syllabusId);
        if (syllabus != null)
        {
            // Populate courses from separate collection
            var courses = await _courseDao.GetBySyllabusIdAsync(syllabusId);
            syllabus.Courses = courses;
        }
        return syllabus;
    }

    public async Task<List<Syllabus>> GetAllAsync()
    {
        var syllabuses = await _syllabusDao.GetAllAsync();

        // Populate courses for each syllabus
        foreach (var syllabus in syllabuses)
        {
            var courses = await _courseDao.GetBySyllabusIdAsync(syllabus.SyllabusId);

            // Populate sessions for each course
            foreach (var course in courses)
            {
                var sessions = await _sessionDao.GetByCourseIdAsync(course.CourseId);
                course.Sessions = sessions;
            }

            syllabus.Courses = courses;
        }

        return syllabuses;
    }

    public async Task<List<Syllabus>> GetByCreatorIdAsync(Guid creatorId)
    {
        var syllabuses = await _syllabusDao.GetByCreatorIdAsync(creatorId);

        // Populate courses for each syllabus
        foreach (var syllabus in syllabuses)
        {
            var courses = await _courseDao.GetBySyllabusIdAsync(syllabus.SyllabusId);
            syllabus.Courses = courses;
        }

        return syllabuses;
    }

    public async Task<List<Syllabus>> GetBySubjectAsync(string subject)
    {
        var syllabuses = await _syllabusDao.GetBySubjectAsync(subject);

        // Populate courses for each syllabus
        foreach (var syllabus in syllabuses)
        {
            var courses = await _courseDao.GetBySyllabusIdAsync(syllabus.SyllabusId);
            syllabus.Courses = courses;
        }

        return syllabuses;
    }

    public async Task<List<Syllabus>> GetByGradeLevelAsync(string gradeLevel)
    {
        var syllabuses = await _syllabusDao.GetByGradeLevelAsync(gradeLevel);

        // Populate courses for each syllabus
        foreach (var syllabus in syllabuses)
        {
            var courses = await _courseDao.GetBySyllabusIdAsync(syllabus.SyllabusId);
            syllabus.Courses = courses;
        }

        return syllabuses;
    }

    public async Task<List<Syllabus>> GetByStatusAsync(string status)
    {
        var syllabuses = await _syllabusDao.GetByStatusAsync(status);

        // Populate courses for each syllabus
        foreach (var syllabus in syllabuses)
        {
            var courses = await _courseDao.GetBySyllabusIdAsync(syllabus.SyllabusId);
            syllabus.Courses = courses;
        }

        return syllabuses;
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