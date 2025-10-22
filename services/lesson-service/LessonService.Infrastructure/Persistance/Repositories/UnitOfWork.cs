using LessonService.Domain.Entities;
using LessonService.Domain.IDAOs;
using LessonService.Domain.Interfaces;
using LessonService.Infrastructure.Persistance.DAOs;
using LessonService.Infrastructure.Persistance.DBContext;

namespace LessonService.Infrastructure.Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly LessonDbContext _context;

        // Lazy load cả DAOs và Repositories
        private IGenericDAO<Syllabus>? _genericDAOSyllabus;
        private IGenericDAO<Course>? _genericDAOCourse;
        private IGenericDAO<Session>? _genericDAOSession;
        private IGenericDAO<Activity>? _genericDAOActivity;
        private IGenericDAO<LessonContext>? _genericDAOLessonContext;

        private IGenericRepository<Syllabus>? _syllabusRepository;
        private IGenericRepository<Course>? _courseRepository;
        private IGenericRepository<Session>? _sessionRepository;
        private IGenericRepository<Activity>? _activityRepository;
        private IGenericRepository<LessonContext>? _lessonContextRepository;

        public UnitOfWork(LessonDbContext context)
        {
            _context = context;
        }

        // ✅ Lazy load DAOs chỉ khi cần
        public IGenericRepository<Syllabus> SyllabusRepository => 
            _syllabusRepository ??= new GenericRepository<Syllabus>(
                _genericDAOSyllabus ??= new GenericDAO<Syllabus>(_context)
            );

        public IGenericRepository<Course> CourseRepository => 
            _courseRepository ??= new GenericRepository<Course>(
                _genericDAOCourse ??= new GenericDAO<Course>(_context)
            );

        public IGenericRepository<Session> SessionRepository => 
            _sessionRepository ??= new GenericRepository<Session>(
                _genericDAOSession ??= new GenericDAO<Session>(_context)
            );

        public IGenericRepository<Activity> ActivityRepository => 
            _activityRepository ??= new GenericRepository<Activity>(
                _genericDAOActivity ??= new GenericDAO<Activity>(_context)
            );

        public IGenericRepository<LessonContext> LessonContextRepository => 
            _lessonContextRepository ??= new GenericRepository<LessonContext>(
                _genericDAOLessonContext ??= new GenericDAO<LessonContext>(_context)
            );

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                    // ✅ Nullify tất cả references
                    _genericDAOSyllabus = null;
                    _genericDAOCourse = null;
                    _genericDAOSession = null;
                    _genericDAOActivity = null;
                    _genericDAOLessonContext = null;
                    
                    _syllabusRepository = null;
                    _courseRepository = null;
                    _sessionRepository = null;
                    _activityRepository = null;
                    _lessonContextRepository = null;
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
