using AutoMapper;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.Question.UpdateQuestion
{
    public class UpdateQuestionCommandHandler : ICommandHandler<UpdateQuestionCommand, Guid>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;

        public UpdateQuestionCommandHandler(IQuestionRepository questionRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<Guid>> Handle(UpdateQuestionCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var existingQuestion = await _questionRepository.GetByIdAsync(command.QuestionId);
                if (existingQuestion == null)
                {
                    return ApiResponse<Guid>.FailureResponse("Question not found", 404);
                }

                existingQuestion.Title = command.Title;
                existingQuestion.Body = command.Body;
                existingQuestion.QuestionType = command.QuestionType;
                existingQuestion.Metadata = command.Metadata;
                existingQuestion.Tags = command.Tags;
                existingQuestion.Version = command.Version;
                existingQuestion.IsPublished = command.IsPublished;
                existingQuestion.QuestionBankId = command.QuestionBankId;
                existingQuestion.AuthorId = command.AuthorId;
                existingQuestion.UpdatedAt = DateTime.UtcNow;

                await _questionRepository.UpdateAsync(existingQuestion);

                return ApiResponse<Guid>.SuccessResponse(
                    existingQuestion.QuestionId,
                    "Question updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<Guid>.FailureResponse($"Error updating question: {ex.Message}", 500);
            }
        }
    }
}

