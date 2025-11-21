using LessonServiceQuery.Domain.Entities;
namespace LessonServiceQuery.Domain.IDAOs;
public interface ICourseDao
{
    Task<Course?> GetByIdAsync(Guid courseId);
    Task<List<Course>> GetBySyllabusIdAsync(Guid syllabusId);
    Task<List<Course>> GetAllAsync();
    Task<Course> CreateAsync(Guid syllabusId, Course course);
    Task<Course> UpdateAsync(Course course);
    Task DeleteAsync(Guid courseId);
    Task<bool> ExistsAsync(Guid courseId);
    Task DeactivateAllByIdAsync(Guid courseId);
}
