using LessonService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonService.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Syllabus> SyllabusRepository { get; }
        IGenericRepository<Course> CourseRepository { get; }
        IGenericRepository<Session> SessionRepository { get; }
        IGenericRepository<Activity> ActivityRepository { get; }
        IGenericRepository<LessonContext> LessonContextRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
