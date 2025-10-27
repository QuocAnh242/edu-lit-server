using AutoMapper;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.QuestionOption.GetQuestionOptionById
{
    public class GetQuestionOptionByIdQueryHandler : IQueryHandler<GetQuestionOptionByIdQuery, QuestionOptionDto>
    {
        private readonly IQuestionOptionRepository _questionOptionRepository;
        private readonly IMapper _mapper;

        public GetQuestionOptionByIdQueryHandler(IQuestionOptionRepository questionOptionRepository, IMapper mapper)
        {
            _questionOptionRepository = questionOptionRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<QuestionOptionDto>> Handle(GetQuestionOptionByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var questionOption = await _questionOptionRepository.GetByIdAsync(query.QuestionOptionId);
                if (questionOption == null)
                {
                    return ApiResponse<QuestionOptionDto>.FailureResponse("Question option not found", 404);
                }

                var questionOptionDto = _mapper.Map<QuestionOptionDto>(questionOption);
                return ApiResponse<QuestionOptionDto>.SuccessResponse(questionOptionDto, "Question option retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<QuestionOptionDto>.FailureResponse($"Error retrieving question option: {ex.Message}", 500);
            }
        }
    }
}

