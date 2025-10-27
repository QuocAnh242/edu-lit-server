using QuestionService.Application.DTOs;
using QuestionService.Application.DTOs.Request;
using QuestionService.Application.DTOs.Response;
using QuestionService.Application.Services.Interfaces;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces;
using QuestionService.Domain.Enums;

namespace QuestionService.Application.Services
{
    public class QuestionBankService : IQuestionBankService
    {
        private readonly IQuestionBankRepository _questionBankRepository;

        public QuestionBankService(IQuestionBankRepository questionBankRepository)
        {
            _questionBankRepository = questionBankRepository;
        }

        public async Task<ApiResponse<QuestionBankDto>> GetByIdAsync(Guid questionBanksId)
        {
            try
            {
                var questionBank = await _questionBankRepository.GetByIdAsync(questionBanksId);
                if (questionBank == null)
                {
                    return ApiResponse<QuestionBankDto>.FailureResponse("Question bank not found", 404);
                }

                var questionBankDto = MapToQuestionBankDto(questionBank);
                return ApiResponse<QuestionBankDto>.SuccessResponse(questionBankDto, "Question bank retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<QuestionBankDto>.FailureResponse($"Error retrieving question bank: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<QuestionBankDto>>> GetAllAsync()
        {
            try
            {
                var questionBanks = await _questionBankRepository.GetAllAsync();
                var questionBankDtos = questionBanks.Select(MapToQuestionBankDto);
                return ApiResponse<IEnumerable<QuestionBankDto>>.SuccessResponse(questionBankDtos, "Question banks retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<QuestionBankDto>>.FailureResponse($"Error retrieving question banks: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<QuestionBankDto>>> GetByOwnerIdAsync(Guid ownerId)
        {
            try
            {
                var questionBanks = await _questionBankRepository.GetByOwnerIdAsync(ownerId);
                var questionBankDtos = questionBanks.Select(MapToQuestionBankDto);
                return ApiResponse<IEnumerable<QuestionBankDto>>.SuccessResponse(questionBankDtos, "Question banks retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<QuestionBankDto>>.FailureResponse($"Error retrieving question banks: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<QuestionBankDto>>> GetBySubjectAsync(string subject)
        {
            try
            {
                var questionBanks = await _questionBankRepository.GetBySubjectAsync(subject);
                var questionBankDtos = questionBanks.Select(MapToQuestionBankDto);
                return ApiResponse<IEnumerable<QuestionBankDto>>.SuccessResponse(questionBankDtos, "Question banks retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<QuestionBankDto>>.FailureResponse($"Error retrieving question banks: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<QuestionBankDto>> CreateAsync(CreateQuestionBankRequest request)
        {
            try
            {
                var questionBank = new QuestionBank
                {
                    QuestionBanksId = Guid.NewGuid(),
                    Title = request.Title,
                    Description = request.Description,
                    Subject = request.Subject,
                    OwnerId = request.OwnerId
                };

                var createdQuestionBank = await _questionBankRepository.CreateAsync(questionBank);
                var questionBankDto = MapToQuestionBankDto(createdQuestionBank);

                return ApiResponse<QuestionBankDto>.SuccessResponse(questionBankDto, "Question bank created successfully", 201);
            }
            catch (Exception ex)
            {
                return ApiResponse<QuestionBankDto>.FailureResponse($"Error creating question bank: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<QuestionBankDto>> UpdateAsync(Guid questionBanksId, CreateQuestionBankRequest request)
        {
            try
            {
                var existingQuestionBank = await _questionBankRepository.GetByIdAsync(questionBanksId);
                if (existingQuestionBank == null)
                {
                    return ApiResponse<QuestionBankDto>.FailureResponse("Question bank not found", 404);
                }

                existingQuestionBank.Title = request.Title;
                existingQuestionBank.Description = request.Description;
                existingQuestionBank.Subject = request.Subject;
                existingQuestionBank.OwnerId = request.OwnerId;

                var updatedQuestionBank = await _questionBankRepository.UpdateAsync(existingQuestionBank);
                var questionBankDto = MapToQuestionBankDto(updatedQuestionBank);

                return ApiResponse<QuestionBankDto>.SuccessResponse(questionBankDto, "Question bank updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<QuestionBankDto>.FailureResponse($"Error updating question bank: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid questionBanksId)
        {
            try
            {
                var result = await _questionBankRepository.DeleteAsync(questionBanksId);
                if (!result)
                {
                    return ApiResponse<bool>.FailureResponse("Question bank not found", 404);
                }

                return ApiResponse<bool>.SuccessResponse(true, "Question bank deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResponse($"Error deleting question bank: {ex.Message}", 500);
            }
        }

        private static QuestionBankDto MapToQuestionBankDto(QuestionBank questionBank)
        {
            return new QuestionBankDto
            {
                QuestionBanksId = questionBank.QuestionBanksId,
                Title = questionBank.Title,
                Description = questionBank.Description,
                Subject = questionBank.Subject,
                CreatedAt = questionBank.CreatedAt,
                OwnerId = questionBank.OwnerId,
                Questions = questionBank.Questions?.Select(q => new QuestionDto
                {
                    QuestionId = q.QuestionId,
                    Title = q.Title,
                    Body = q.Body,
                    QuestionType = q.QuestionType,
                    Metadata = q.Metadata,
                    Tags = q.Tags,
                    Version = q.Version,
                    CreatedAt = q.CreatedAt,
                    UpdatedAt = q.UpdatedAt,
                    IsPublished = q.IsPublished,
                    QuestionBankId = q.QuestionBankId,
                    AuthorId = q.AuthorId
                }).ToList() ?? new List<QuestionDto>()
            };
        }
    }
}

