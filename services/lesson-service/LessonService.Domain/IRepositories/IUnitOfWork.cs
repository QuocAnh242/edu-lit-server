using LessonService.Domain.Entities;
using LessonService.Domain.IRepositories;

namespace LessonService.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Syllabus> SyllabusRepository { get; }
        IGenericRepository<Course> CourseRepository { get; }
        IGenericRepository<Session> SessionRepository { get; }
        IGenericRepository<Activity> ActivityRepository { get; }
        IGenericRepository<LessonContext> LessonContextRepository { get; }
        IOutboxRepository OutboxRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
