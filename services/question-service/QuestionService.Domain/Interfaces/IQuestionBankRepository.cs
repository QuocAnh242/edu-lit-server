using QuestionService.Domain.Entities;

namespace QuestionService.Domain.Interfaces
{
    public interface IQuestionBankRepository
    {
        Task<QuestionBank?> GetByIdAsync(Guid questionBanksId);
        Task<IEnumerable<QuestionBank>> GetAllAsync();
        Task<IEnumerable<QuestionBank>> GetByOwnerIdAsync(Guid ownerId);
        Task<IEnumerable<QuestionBank>> GetBySubjectAsync(string subject);
        Task<QuestionBank> CreateAsync(QuestionBank questionBank);
        Task<QuestionBank> UpdateAsync(QuestionBank questionBank);
        Task<bool> DeleteAsync(Guid questionBanksId);
        Task<bool> ExistsAsync(Guid questionBanksId);
    }
}

