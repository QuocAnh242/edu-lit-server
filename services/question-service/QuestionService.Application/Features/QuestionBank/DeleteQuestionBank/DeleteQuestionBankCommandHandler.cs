using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.QuestionBank.DeleteQuestionBank
{
    public class DeleteQuestionBankCommandHandler : ICommandHandler<DeleteQuestionBankCommand, bool>
    {
        private readonly IQuestionBankRepository _questionBankRepository;

        public DeleteQuestionBankCommandHandler(IQuestionBankRepository questionBankRepository)
        {
            _questionBankRepository = questionBankRepository;
        }

        public async Task<ApiResponse<bool>> Handle(DeleteQuestionBankCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _questionBankRepository.DeleteAsync(command.QuestionBankId);
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
    }
}

