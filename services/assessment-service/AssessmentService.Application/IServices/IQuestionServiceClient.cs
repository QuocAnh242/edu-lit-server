using AssessmentService.Application.DTOs;

namespace AssessmentService.Application.IServices
{
    public interface IQuestionServiceClient
    {
        Task<QuestionDto?> GetQuestionByIdAsync(Guid questionId, CancellationToken cancellationToken = default);
        Task<QuestionOptionDto?> GetQuestionOptionByIdAsync(Guid optionId, CancellationToken cancellationToken = default);
        Task<List<QuestionOptionDto>> GetQuestionOptionsByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken = default);
        Task<bool> ValidateQuestionIdsAsync(List<Guid> questionIds, CancellationToken cancellationToken = default);
    }
}

