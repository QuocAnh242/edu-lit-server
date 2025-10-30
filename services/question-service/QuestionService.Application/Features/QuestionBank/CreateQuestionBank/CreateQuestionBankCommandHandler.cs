using AutoMapper;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.QuestionBank.CreateQuestionBank
{
    public class CreateQuestionBankCommandHandler : ICommandHandler<CreateQuestionBankCommand, Guid>
    {
        private readonly IQuestionBankRepository _questionBankRepository;
        private readonly IMapper _mapper;

        public CreateQuestionBankCommandHandler(IQuestionBankRepository questionBankRepository, IMapper mapper)
        {
            _questionBankRepository = questionBankRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<Guid>> Handle(CreateQuestionBankCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var questionBank = _mapper.Map<Domain.Entities.QuestionBank>(command);
                questionBank.QuestionBanksId = Guid.NewGuid();
                questionBank.CreatedAt = DateTime.UtcNow;

                var createdQuestionBank = await _questionBankRepository.CreateAsync(questionBank);

                return ApiResponse<Guid>.SuccessResponse(
                    createdQuestionBank.QuestionBanksId,
                    "Question bank created successfully",
                    201);
            }
            catch (Exception ex)
            {
                return ApiResponse<Guid>.FailureResponse($"Error creating question bank: {ex.Message}", 500);
            }
        }
    }
}

