using QuestionService.Application.DTOs;
using QuestionService.Application.DTOs.Request;
using QuestionService.Application.DTOs.Response;
using QuestionService.Application.Services.Interfaces;
using QuestionService.Domain.Entities;
using QuestionService.Domain.Interfaces;
using QuestionService.Domain.Enums;

namespace QuestionService.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IQuestionOptionRepository _questionOptionRepository;

        public QuestionService(
            IQuestionRepository questionRepository,
            IQuestionOptionRepository questionOptionRepository)
        {
            _questionRepository = questionRepository;
            _questionOptionRepository = questionOptionRepository;
        }

        public async Task<ApiResponse<QuestionDto>> GetByIdAsync(Guid questionId)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(questionId);
                if (question == null)
                {
                    return ApiResponse<QuestionDto>.FailureResponse("Question not found", 404);
                }

                var questionDto = MapToQuestionDto(question);
                return ApiResponse<QuestionDto>.SuccessResponse(questionDto, "Question retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<QuestionDto>.FailureResponse($"Error retrieving question: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<QuestionDto>>> GetAllAsync()
        {
            try
            {
                var questions = await _questionRepository.GetAllAsync();
                var questionDtos = questions.Select(MapToQuestionDto);
                return ApiResponse<IEnumerable<QuestionDto>>.SuccessResponse(questionDtos, "Questions retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<QuestionDto>>.FailureResponse($"Error retrieving questions: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<QuestionDto>>> GetByQuestionBankIdAsync(Guid questionBankId)
        {
            try
            {
                var questions = await _questionRepository.GetByQuestionBankIdAsync(questionBankId);
                var questionDtos = questions.Select(MapToQuestionDto);
                return ApiResponse<IEnumerable<QuestionDto>>.SuccessResponse(questionDtos, "Questions retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<QuestionDto>>.FailureResponse($"Error retrieving questions: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<QuestionDto>>> GetByAuthorIdAsync(Guid authorId)
        {
            try
            {
                var questions = await _questionRepository.GetByAuthorIdAsync(authorId);
                var questionDtos = questions.Select(MapToQuestionDto);
                return ApiResponse<IEnumerable<QuestionDto>>.SuccessResponse(questionDtos, "Questions retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<QuestionDto>>.FailureResponse($"Error retrieving questions: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<QuestionDto>>> GetByQuestionTypeAsync(QuestionType questionType)
        {
            try
            {
                var questions = await _questionRepository.GetByQuestionTypeAsync(questionType);
                var questionDtos = questions.Select(MapToQuestionDto);
                return ApiResponse<IEnumerable<QuestionDto>>.SuccessResponse(questionDtos, "Questions retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<QuestionDto>>.FailureResponse($"Error retrieving questions: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<IEnumerable<QuestionDto>>> GetPublishedQuestionsAsync()
        {
            try
            {
                var questions = await _questionRepository.GetPublishedQuestionsAsync();
                var questionDtos = questions.Select(MapToQuestionDto);
                return ApiResponse<IEnumerable<QuestionDto>>.SuccessResponse(questionDtos, "Questions retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<QuestionDto>>.FailureResponse($"Error retrieving questions: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<QuestionDto>> CreateAsync(CreateQuestionRequest request)
        {
            try
            {
                var question = new Question
                {
                    QuestionId = Guid.NewGuid(),
                    Title = request.Title,
                    Body = request.Body,
                    QuestionType = request.QuestionType,
                    Metadata = request.Metadata,
                    Tags = request.Tags,
                    Version = request.Version,
                    IsPublished = request.IsPublished,
                    QuestionBankId = request.QuestionBankId,
                    AuthorId = request.AuthorId
                };

                var createdQuestion = await _questionRepository.CreateAsync(question);
                var questionDto = MapToQuestionDto(createdQuestion);

                return ApiResponse<QuestionDto>.SuccessResponse(questionDto, "Question created successfully", 201);
            }
            catch (Exception ex)
            {
                return ApiResponse<QuestionDto>.FailureResponse($"Error creating question: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<QuestionDto>> UpdateAsync(Guid questionId, CreateQuestionRequest request)
        {
            try
            {
                var existingQuestion = await _questionRepository.GetByIdAsync(questionId);
                if (existingQuestion == null)
                {
                    return ApiResponse<QuestionDto>.FailureResponse("Question not found", 404);
                }

                // Update question properties
                existingQuestion.Title = request.Title;
                existingQuestion.Body = request.Body;
                existingQuestion.QuestionType = request.QuestionType;
                existingQuestion.Metadata = request.Metadata;
                existingQuestion.Tags = request.Tags;
                existingQuestion.Version = request.Version;
                existingQuestion.IsPublished = request.IsPublished;
                existingQuestion.QuestionBankId = request.QuestionBankId;
                existingQuestion.AuthorId = request.AuthorId;

                await _questionRepository.UpdateAsync(existingQuestion);
                var questionDto = MapToQuestionDto(existingQuestion);

                return ApiResponse<QuestionDto>.SuccessResponse(questionDto, "Question updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<QuestionDto>.FailureResponse($"Error updating question: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid questionId)
        {
            try
            {
                var result = await _questionRepository.DeleteAsync(questionId);
                if (!result)
                {
                    return ApiResponse<bool>.FailureResponse("Question not found", 404);
                }

                return ApiResponse<bool>.SuccessResponse(true, "Question deleted successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.FailureResponse($"Error deleting question: {ex.Message}", 500);
            }
        }

        private static QuestionDto MapToQuestionDto(Question question)
        {
            return new QuestionDto
            {
                QuestionId = question.QuestionId,
                Title = question.Title,
                Body = question.Body,
                QuestionType = question.QuestionType,
                Metadata = question.Metadata,
                Tags = question.Tags,
                Version = question.Version,
                CreatedAt = question.CreatedAt,
                UpdatedAt = question.UpdatedAt,
                IsPublished = question.IsPublished,
                QuestionBankId = question.QuestionBankId,
                AuthorId = question.AuthorId
            };
        }
    }
}
