using AutoMapper;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.Question.GetQuestionById
{
    public class GetQuestionByIdQueryHandler : IQueryHandler<GetQuestionByIdQuery, QuestionDto>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;

        public GetQuestionByIdQueryHandler(IQuestionRepository questionRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<QuestionDto>> Handle(GetQuestionByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var question = await _questionRepository.GetByIdAsync(query.QuestionId);
                if (question == null)
                {
                    return ApiResponse<QuestionDto>.FailureResponse("Question not found", 404);
                }

                var questionDto = _mapper.Map<QuestionDto>(question);
                return ApiResponse<QuestionDto>.SuccessResponse(questionDto, "Question retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<QuestionDto>.FailureResponse($"Error retrieving question: {ex.Message}", 500);
            }
        }
    }
}

