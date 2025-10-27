using AutoMapper;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.Question.GetQuestionsByAuthorId
{
    public class GetQuestionsByAuthorIdQueryHandler : IQueryHandler<GetQuestionsByAuthorIdQuery, IEnumerable<QuestionDto>>
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly IMapper _mapper;

        public GetQuestionsByAuthorIdQueryHandler(IQuestionRepository questionRepository, IMapper mapper)
        {
            _questionRepository = questionRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<QuestionDto>>> Handle(GetQuestionsByAuthorIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var questions = await _questionRepository.GetByAuthorIdAsync(query.AuthorId);
                var questionDtos = _mapper.Map<IEnumerable<QuestionDto>>(questions);
                return ApiResponse<IEnumerable<QuestionDto>>.SuccessResponse(questionDtos, "Questions retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<QuestionDto>>.FailureResponse($"Error retrieving questions: {ex.Message}", 500);
            }
        }
    }
}

