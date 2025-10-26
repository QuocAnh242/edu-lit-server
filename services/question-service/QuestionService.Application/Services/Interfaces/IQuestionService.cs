using QuestionService.Application.DTOs;
using QuestionService.Application.DTOs.Request;
using QuestionService.Application.DTOs.Response;
using QuestionService.Domain.Enums;

namespace QuestionService.Application.Services.Interfaces
{
    public interface IQuestionService
    {
        Task<ApiResponse<QuestionDto>> GetByIdAsync(Guid questionId);
        Task<ApiResponse<IEnumerable<QuestionDto>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<QuestionDto>>> GetByQuestionBankIdAsync(Guid questionBankId);
        Task<ApiResponse<IEnumerable<QuestionDto>>> GetByAuthorIdAsync(Guid authorId);
        Task<ApiResponse<IEnumerable<QuestionDto>>> GetByQuestionTypeAsync(QuestionType questionType);
        Task<ApiResponse<IEnumerable<QuestionDto>>> GetPublishedQuestionsAsync();
        Task<ApiResponse<QuestionDto>> CreateAsync(CreateQuestionRequest request);
        Task<ApiResponse<QuestionDto>> UpdateAsync(Guid questionId, CreateQuestionRequest request);
        Task<ApiResponse<bool>> DeleteAsync(Guid questionId);
    }

    public interface IQuestionBankService
    {
        Task<ApiResponse<QuestionBankDto>> GetByIdAsync(Guid questionBanksId);
        Task<ApiResponse<IEnumerable<QuestionBankDto>>> GetAllAsync();
        Task<ApiResponse<IEnumerable<QuestionBankDto>>> GetByOwnerIdAsync(Guid ownerId);
        Task<ApiResponse<IEnumerable<QuestionBankDto>>> GetBySubjectAsync(string subject);
        Task<ApiResponse<QuestionBankDto>> CreateAsync(CreateQuestionBankRequest request);
        Task<ApiResponse<QuestionBankDto>> UpdateAsync(Guid questionBanksId, CreateQuestionBankRequest request);
        Task<ApiResponse<bool>> DeleteAsync(Guid questionBanksId);
    }

    public interface IQuestionOptionService
    {
        Task<ApiResponse<QuestionOptionDto>> GetByIdAsync(Guid questionOptionId);
        Task<ApiResponse<IEnumerable<QuestionOptionDto>>> GetByQuestionIdAsync(Guid questionId);
        Task<ApiResponse<QuestionOptionDto>> CreateAsync(CreateQuestionOptionRequest request);
        Task<ApiResponse<QuestionOptionDto>> UpdateAsync(Guid questionOptionId, CreateQuestionOptionRequest request);
        Task<ApiResponse<bool>> DeleteAsync(Guid questionOptionId);
        Task<ApiResponse<bool>> DeleteByQuestionIdAsync(Guid questionId);
    }
}
