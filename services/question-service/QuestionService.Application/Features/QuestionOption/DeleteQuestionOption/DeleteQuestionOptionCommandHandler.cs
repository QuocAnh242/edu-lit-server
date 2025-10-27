using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.QuestionOption.DeleteQuestionOption
{
    public class DeleteQuestionOptionCommandHandler : ICommandHandler<DeleteQuestionOptionCommand, bool>
    {
        private readonly IQuestionOptionRepository _questionOptionRepository;

        public DeleteQuestionOptionCommandHandler(IQuestionOptionRepository questionOptionRepository)
        {
            _questionOptionRepository = questionOptionRepository;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteQuestionOptionCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _questionOptionRepository.DeleteAsync(command.QuestionOptionId);
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
    }
}

