using QuestionService.Application.DTOs;
using QuestionService.Application.DTOs.Request;
using QuestionService.Application.DTOs.Response;
using QuestionService.Application.Services.Interfaces;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Services
{
    public class QuestionOptionService : IQuestionOptionService
    {
        private readonly IQuestionOptionRepository _questionOptionRepository;

        public QuestionOptionService(IQuestionOptionRepository questionOptionRepository)
        {
            _questionOptionRepository = questionOptionRepository;
        }

        public async Task<ApiResponse<QuestionOptionDto>> GetByIdAsync(Guid questionOptionId)
        {
            try
            {
                var questionOption = await _questionOptionRepository.GetByIdAsync(questionOptionId);
                if (questionOption == null)
                {
                    return ApiResponse<QuestionOptionDto>.FailureResponse("Question option not found", 404);
                }

                var questionOptionDto = MapToQuestionOptionDto(questionOption);
                return ApiResponse<QuestionOptionDto>.SuccessResponse(questionOptionDto, "Question option retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<QuestionOptionDto>.FailureResponse($"Error retrieving question option: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<QuestionOptionDto>>> GetByQuestionIdAsync(Guid questionId)
        {
            try
            {
                var questionOptions = await _questionOptionRepository.GetByQuestionIdAsync(questionId);
                var questionOptionDtos = questionOptions.Select(MapToQuestionOptionDto);
                return ApiResponse<IEnumerable<QuestionOptionDto>>.SuccessResponse(questionOptionDtos, "Question options retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<QuestionOptionDto>>.FailureResponse($"Error retrieving question options: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<QuestionOptionDto>> CreateAsync(CreateQuestionOptionRequest request)
        {
            try
            {
                var questionOption = new QuestionOption
                {
                    QuestionOptionId = Guid.NewGuid(),
                    OptionText = request.OptionText,
                    IsCorrect = request.IsCorrect,
                    OrderIdx = request.OrderIdx,
                    QuestionId = request.QuestionId
                };

                var createdQuestionOption = await _questionOptionRepository.CreateAsync(questionOption);
                var questionOptionDto = MapToQuestionOptionDto(createdQuestionOption);

                return ApiResponse<QuestionOptionDto>.SuccessResponse(questionOptionDto, "Question option created successfully", 201);
            }
            catch (Exception ex)
            {
                return ApiResponse<QuestionOptionDto>.FailureResponse($"Error creating question option: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<QuestionOptionDto>> UpdateAsync(Guid questionOptionId, CreateQuestionOptionRequest request)
        {
            try
            {
                var existingQuestionOption = await _questionOptionRepository.GetByIdAsync(questionOptionId);
                if (existingQuestionOption == null)
                {
                    return ApiResponse<QuestionOptionDto>.FailureResponse("Question option not found", 404);
                }

                existingQuestionOption.OptionText = request.OptionText;
                existingQuestionOption.IsCorrect = request.IsCorrect;
                existingQuestionOption.OrderIdx = request.OrderIdx;

                var updatedQuestionOption = await _questionOptionRepository.UpdateAsync(existingQuestionOption);
                var questionOptionDto = MapToQuestionOptionDto(updatedQuestionOption);

                return ApiResponse<QuestionOptionDto>.SuccessResponse(questionOptionDto, "Question option updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<QuestionOptionDto>.FailureResponse($"Error updating question option: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid questionOptionId)
        {
            try
            {
                var result = await _questionOptionRepository.DeleteAsync(questionOptionId);
                if (!result)
                {
                    return ApiResponse<bool>.FailureResponse("Question option not found", 404);
                }

                return ApiResponse<bool>.SuccessResponse(true, "Question option deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResponse($"Error deleting question option: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> DeleteByQuestionIdAsync(Guid questionId)
        {
            try
            {
                var result = await _questionOptionRepository.DeleteByQuestionIdAsync(questionId);
                if (!result)
                {
                    return ApiResponse<bool>.FailureResponse("No question options found for the given question", 404);
                }

                return ApiResponse<bool>.SuccessResponse(true, "Question options deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResponse($"Error deleting question options: {ex.Message}", 500);
            }
        }

        private static QuestionOptionDto MapToQuestionOptionDto(QuestionOption questionOption)
        {
            return new QuestionOptionDto
            {
                QuestionOptionId = questionOption.QuestionOptionId,
                OptionText = questionOption.OptionText,
                IsCorrect = questionOption.IsCorrect,
                OrderIdx = questionOption.OrderIdx,
                QuestionId = questionOption.QuestionId
            };
        }
    }
}

