using QuestionService.Domain.Entities;

namespace QuestionService.Domain.Interfaces
{
    public interface IQuestionRepository
    {
        Task<Question?> GetByIdAsync(Guid questionId);
        Task<IEnumerable<Question>> GetAllAsync();
        Task<IEnumerable<Question>> GetByQuestionBankIdAsync(Guid questionBankId);
        Task<IEnumerable<Question>> GetByAuthorIdAsync(Guid authorId);
        Task<IEnumerable<Question>> GetByQuestionTypeAsync(string questionType);
        Task<IEnumerable<Question>> GetPublishedQuestionsAsync();
        Task<Question> CreateAsync(Question question);
        Task<Question> UpdateAsync(Question question);
        Task<bool> DeleteAsync(Guid questionId);
        Task<bool> ExistsAsync(Guid questionId);
    }
}
