using LessonServiceQuery.Domain.Entities;
namespace LessonServiceQuery.Domain.Repositories;
public interface ICourseRepository
{
    Task<Course?> GetByIdAsync(Guid courseId);
    Task<List<Course>> GetBySyllabusIdAsync(Guid syllabusId);
    Task<Course> CreateAsync(Course course);
    Task<Course> UpdateAsync(Course course);
    Task DeleteAsync(Guid courseId);
    Task<bool> ExistsAsync(Guid courseId);
}
