using AutoMapper;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.Question.CreateQuestion
{
    public class CreateQuestionCommandHandler : ICommandHandler<CreateQuestionCommand, Guid>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;

        public CreateQuestionCommandHandler(IQuestionRepository questionRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<Guid>> Handle(CreateQuestionCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var question = _mapper.Map<Domain.Entities.Question>(command);
                question.QuestionId = Guid.NewGuid();
                question.CreatedAt = DateTime.UtcNow;

                var createdQuestion = await _questionRepository.CreateAsync(question);

                return ApiResponse<Guid>.SuccessResponse(
                    createdQuestion.QuestionId,
                    "Question created successfully",
                    201);
            }
            catch (Exception ex)
            {
                return ApiResponse<Guid>.FailureResponse($"Error creating question: {ex.Message}", 500);
            }
        }
    }
}

