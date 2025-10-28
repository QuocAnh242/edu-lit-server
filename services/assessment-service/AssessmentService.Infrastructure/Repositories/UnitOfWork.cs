using AssessmentService.Domain.Entities;
using AssessmentService.Domain.IDAOs;
using AssessmentService.Domain.Interfaces;
using AssessmentService.Infrastructure.Persistance.DBContext;
using AssessmentService.Infrastructure.Persistance.Repositories;
using AssessmentService.Domain.Interfaces;
using AssessmentService.Infrastructure.Persistance.DAOs;

namespace AssessmentService.Infrastructure.Persistance.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AssessmentDbContext _context;

        // Lazy load cả DAOs và Repositories
        private IGenericDAO<Assessment>? _genericDAOAssessment;
        private IGenericDAO<AssessmentAnswer>? _genericDAOAssessmentAnswer;
        private IGenericDAO<AssessmentQuestion>? _genericDAOAssessmentQuestion;
        private IGenericDAO<AssignmentAttempt>? _genericDAOAssignmentAttempt;
        private IGenericDAO<GradingFeedback>? _genericDAOGradingFeedback;

        private IGenericRepository<Assessment>? _assessmentRepository;
        private IGenericRepository<AssessmentAnswer>? _assessmentAnswerRepository;
        private IGenericRepository<AssessmentQuestion>? _assessmentQuestionRepository;
        private IGenericRepository<AssignmentAttempt>? _assignmentAttemptRepository;
        private IGenericRepository<GradingFeedback>? _gradingFeedbackRepository;

        public UnitOfWork(AssessmentDbContext context)
        {
            _context = context;
        }

        // ✅ Lazy load DAOs chỉ khi cần
        public IGenericRepository<Assessment> AssessmentRepository =>
            _assessmentRepository ??= new GenericRepository<Assessment>(
                _genericDAOAssessment ??= new GenericDAO<Assessment>(_context)
            );

        public IGenericRepository<AssessmentAnswer> AssessmentAnswerRepository =>
            _assessmentAnswerRepository ??= new GenericRepository<AssessmentAnswer>(
                _genericDAOAssessmentAnswer ??= new GenericDAO<AssessmentAnswer>(_context)
            );

        public IGenericRepository<AssessmentQuestion> AssessmentQuestionRepository =>
            _assessmentQuestionRepository ??= new GenericRepository<AssessmentQuestion>(
                _genericDAOAssessmentQuestion ??= new GenericDAO<AssessmentQuestion>(_context)
            );

        public IGenericRepository<AssignmentAttempt> AssignmentAttemptRepository =>
            _assignmentAttemptRepository ??= new GenericRepository<AssignmentAttempt>(
                _genericDAOAssignmentAttempt ??= new GenericDAO<AssignmentAttempt>(_context)
            );

        public IGenericRepository<GradingFeedback> GradingFeedbackRepository =>
            _gradingFeedbackRepository ??= new GenericRepository<GradingFeedback>(
                _genericDAOGradingFeedback ??= new GenericDAO<GradingFeedback>(_context)
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
                    _genericDAOAssessment = null;
                    _genericDAOAssessmentAnswer = null;
                    _genericDAOAssessmentQuestion = null;
                    _genericDAOAssignmentAttempt = null;
                    _genericDAOGradingFeedback = null;

                    _assessmentRepository = null;
                    _assessmentAnswerRepository = null;
                    _assessmentQuestionRepository = null;
                    _assignmentAttemptRepository = null;
                    _gradingFeedbackRepository = null;
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
