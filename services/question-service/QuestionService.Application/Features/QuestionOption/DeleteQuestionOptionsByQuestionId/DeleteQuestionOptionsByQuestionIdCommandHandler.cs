using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.QuestionOption.DeleteQuestionOptionsByQuestionId
{
    public class DeleteQuestionOptionsByQuestionIdCommandHandler : ICommandHandler<DeleteQuestionOptionsByQuestionIdCommand, bool>
    {
        private readonly IQuestionOptionRepository _questionOptionRepository;

        public DeleteQuestionOptionsByQuestionIdCommandHandler(IQuestionOptionRepository questionOptionRepository)
        {
            _questionOptionRepository = questionOptionRepository;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteQuestionOptionsByQuestionIdCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _questionOptionRepository.DeleteByQuestionIdAsync(command.QuestionId);
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
    }
}

