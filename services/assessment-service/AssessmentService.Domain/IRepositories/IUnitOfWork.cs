using AssessmentService.Domain.Entities;
using AssessmentService.Domain.Interfaces;

namespace AssessmentService.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<Assessment> AssessmentRepository { get; }
        IGenericRepository<AssessmentAnswer> AssessmentAnswerRepository { get; }
        IGenericRepository<AssessmentQuestion> AssessmentQuestionRepository { get; }
        IGenericRepository<AssignmentAttempt> AssignmentAttemptRepository { get; }
        IGenericRepository<GradingFeedback> GradingFeedbackRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
