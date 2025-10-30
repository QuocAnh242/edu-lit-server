using AutoMapper;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.Question.GetPublishedQuestions
{
    public class GetPublishedQuestionsQueryHandler : IQueryHandler<GetPublishedQuestionsQuery, IEnumerable<QuestionDto>>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;

        public GetPublishedQuestionsQueryHandler(IQuestionRepository questionRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<QuestionDto>>> Handle(GetPublishedQuestionsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var questions = await _questionRepository.GetPublishedQuestionsAsync();
                var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);
                return ApiResponse<IEnumerable<QuestionDto>>.SuccessResponse(questionDtos, "Published questions retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<QuestionDto>>.FailureResponse($"Error retrieving published questions: {ex.Message}", 500);
            }
        }
    }
}

