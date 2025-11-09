using AutoMapper;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.QuestionBank.UpdateQuestionBank
{
    public class UpdateQuestionBankCommandHandler : ICommandHandler<UpdateQuestionBankCommand, Guid>
    {
        private readonly IQuestionBankRepository _questionBankRepository;
        private readonly IMapper _mapper;

        public UpdateQuestionBankCommandHandler(IQuestionBankRepository questionBankRepository, IMapper mapper)
        {
            _questionBankRepository = questionBankRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<Guid>> Handle(UpdateQuestionBankCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var existingQuestionBank = await _questionBankRepository.GetByIdAsync(command.QuestionBankId);
                if (existingQuestionBank == null)
                {
                    return ApiResponse<Guid>.FailureResponse("Question bank not found", 404);
                }

                existingQuestionBank.Title = command.Title;
                existingQuestionBank.Description = command.Description;
                existingQuestionBank.Subject = command.Subject;
                existingQuestionBank.OwnerId = command.OwnerId;

                await _questionBankRepository.UpdateAsync(existingQuestionBank);

                return ApiResponse<Guid>.SuccessResponse(
                    existingQuestionBank.QuestionBanksId,
                    "Question bank updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<Guid>.FailureResponse($"Error updating question bank: {ex.Message}", 500);
            }
        }
    }
}

