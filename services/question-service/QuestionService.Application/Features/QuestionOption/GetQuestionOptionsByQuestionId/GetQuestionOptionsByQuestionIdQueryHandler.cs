using AutoMapper;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.QuestionOption.GetQuestionOptionsByQuestionId
{
    public class GetQuestionOptionsByQuestionIdQueryHandler : IQueryHandler<GetQuestionOptionsByQuestionIdQuery, IEnumerable<QuestionOptionDto>>
    {
        private readonly IQuestionOptionRepository _questionOptionRepository;
        private readonly IMapper _mapper;

        public GetQuestionOptionsByQuestionIdQueryHandler(IQuestionOptionRepository questionOptionRepository, IMapper mapper)
        {
            _questionOptionRepository = questionOptionRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<QuestionOptionDto>>> Handle(GetQuestionOptionsByQuestionIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var questionOptions = await _questionOptionRepository.GetByQuestionIdAsync(query.QuestionId);
                var questionOptionDtos = _mapper.Map<IEnumerable<QuestionOptionDto>>(questionOptions);
                return ApiResponse<IEnumerable<QuestionOptionDto>>.SuccessResponse(questionOptionDtos, "Question options retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<QuestionOptionDto>>.FailureResponse($"Error retrieving question options: {ex.Message}", 500);
            }
        }
    }
}

