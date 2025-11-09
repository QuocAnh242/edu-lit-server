using AutoMapper;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.QuestionOption.UpdateQuestionOption
{
    public class UpdateQuestionOptionCommandHandler : ICommandHandler<UpdateQuestionOptionCommand, Guid>
    {
        private readonly IQuestionOptionRepository _questionOptionRepository;
        private readonly IMapper _mapper;

        public UpdateQuestionOptionCommandHandler(IQuestionOptionRepository questionOptionRepository, IMapper mapper)
        {
            _questionOptionRepository = questionOptionRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<Guid>> Handle(UpdateQuestionOptionCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var existingQuestionOption = await _questionOptionRepository.GetByIdAsync(command.QuestionOptionId);
                if (existingQuestionOption == null)
                {
                    return ApiResponse<Guid>.FailureResponse("Question option not found", 404);
                }

                existingQuestionOption.OptionText = command.OptionText;
                existingQuestionOption.IsCorrect = command.IsCorrect;
                existingQuestionOption.OrderIdx = command.OrderIdx;

                await _questionOptionRepository.UpdateAsync(existingQuestionOption);

                return ApiResponse<Guid>.SuccessResponse(
                    existingQuestionOption.QuestionOptionId,
                    "Question option updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<Guid>.FailureResponse($"Error updating question option: {ex.Message}", 500);
            }
        }
    }
}

