using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.Question.DeleteQuestion
{
    public class DeleteQuestionCommandHandler : ICommandHandler<DeleteQuestionCommand, bool>
    {
        private readonly IQuestionRepository _questionRepository;

        public DeleteQuestionCommandHandler(IQuestionRepository questionRepository)
        {
            _questionRepository = questionRepository;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteQuestionCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _questionRepository.DeleteAsync(command.QuestionId);
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
    }
}

