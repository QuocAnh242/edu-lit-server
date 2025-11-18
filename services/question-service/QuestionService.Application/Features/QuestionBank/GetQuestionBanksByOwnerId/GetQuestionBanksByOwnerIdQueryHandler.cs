using AutoMapper;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;
using QuestionService.Domain.Commons;
using QuestionService.Domain.Interfaces;

namespace QuestionService.Application.Features.QuestionBank.GetQuestionBanksByOwnerId
{
    public class GetQuestionBanksByOwnerIdQueryHandler : IQueryHandler<GetQuestionBanksByOwnerIdQuery, IEnumerable<QuestionBankDto>>
    {
        private readonly IQuestionBankRepository _questionBankRepository;
        private readonly IMapper _mapper;

        public GetQuestionBanksByOwnerIdQueryHandler(IQuestionBankRepository questionBankRepository, IMapper mapper)
        {
            _questionBankRepository = questionBankRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<QuestionBankDto>>> Handle(GetQuestionBanksByOwnerIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var questionBanks = await _questionBankRepository.GetByOwnerIdAsync(query.OwnerId);
                var questionBankDtos = _mapper.Map<IEnumerable<QuestionBankDto>>(questionBanks);
                return ApiResponse<IEnumerable<QuestionBankDto>>.SuccessResponse(questionBankDtos, "Question banks retrieved successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<QuestionBankDto>>.FailureResponse($"Error retrieving question banks: {ex.Message}", 500);
            }
        }
    }
}

