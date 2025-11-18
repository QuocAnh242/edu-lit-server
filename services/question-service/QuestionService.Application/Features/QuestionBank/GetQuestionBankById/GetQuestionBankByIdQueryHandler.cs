using AutoMapper;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.QuestionBank.GetQuestionBankById
{
    public class GetQuestionBankByIdQueryHandler : IQueryHandler<GetQuestionBankByIdQuery, QuestionBankDto>
    {
        private readonly IQuestionBankRepository _questionBankRepository;
        private readonly IMapper _mapper;

        public GetQuestionBankByIdQueryHandler(IQuestionBankRepository questionBankRepository, IMapper mapper)
        {
            _questionBankRepository = questionBankRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<QuestionBankDto>> Handle(GetQuestionBankByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var questionBank = await _questionBankRepository.GetByIdAsync(query.QuestionBankId);
                if (questionBank == null)
                {
                    return ApiResponse<QuestionBankDto>.FailureResponse("Question bank not found", 404);
                }

                var questionBankDto = _mapper.Map<QuestionBankDto>(questionBank);
                return ApiResponse<QuestionBankDto>.SuccessResponse(questionBankDto, "Question bank retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<QuestionBankDto>.FailureResponse($"Error retrieving question bank: {ex.Message}", 500);
            }
        }
    }
}

