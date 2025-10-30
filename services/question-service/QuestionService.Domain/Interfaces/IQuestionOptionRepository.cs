using QuestionService.Domain.Entities;

namespace QuestionService.Domain.Interfaces
{
    public interface IQuestionOptionRepository
    {
        Task<QuestionOption?> GetByIdAsync(Guid questionOptionId);
        Task<IEnumerable<QuestionOption>> GetAllAsync();
        Task<IEnumerable<QuestionOption>> GetByQuestionIdAsync(Guid questionId);
        Task<QuestionOption> CreateAsync(QuestionOption questionOption);
        Task<QuestionOption> UpdateAsync(QuestionOption questionOption);
        Task<bool> DeleteAsync(Guid questionOptionId);
        Task<bool> DeleteByQuestionIdAsync(Guid questionId);
        Task<bool> ExistsAsync(Guid questionOptionId);
    }
}




