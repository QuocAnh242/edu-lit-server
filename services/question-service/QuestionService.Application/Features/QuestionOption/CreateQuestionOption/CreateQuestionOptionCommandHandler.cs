using AutoMapper;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.QuestionOption.CreateQuestionOption
{
    public class CreateQuestionOptionCommandHandler : ICommandHandler<CreateQuestionOptionCommand, Guid>
    {
        private readonly IQuestionOptionRepository _questionOptionRepository;
        private readonly IMapper _mapper;

        public CreateQuestionOptionCommandHandler(IQuestionOptionRepository questionOptionRepository, IMapper mapper)
        {
            _questionOptionRepository = questionOptionRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<Guid>> Handle(CreateQuestionOptionCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var questionOption = _mapper.Map<Domain.Entities.QuestionOption>(command);
                questionOption.QuestionOptionId = Guid.NewGuid();

                var createdQuestionOption = await _questionOptionRepository.CreateAsync(questionOption);

                return ApiResponse<Guid>.SuccessResponse(
                    createdQuestionOption.QuestionOptionId,
                    "Question option created successfully",
                    201);
            }
            catch (Exception ex)
            {
                return ApiResponse<Guid>.FailureResponse($"Error creating question option: {ex.Message}", 500);
            }
        }
    }
}

