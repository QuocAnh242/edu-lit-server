using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Application.Features.AssessmentAnswer.GetAllAssessmentAnswerByAttemptId
{
    public class GetAllAssessmentAnswerByAttemptIdQueryHandler : IQueryHandler<GetAllAssessmentAnswerByAttemptIdQuery, List<GetAllAssessmentAnswerByAttemptIdResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(10);

        public GetAllAssessmentAnswerByAttemptIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<List<GetAllAssessmentAnswerByAttemptIdResponse>>> Handle(GetAllAssessmentAnswerByAttemptIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                string cacheKey = $"assessmentAnswers:attemptId:{query.Id}";
                // 1. Kiểm tra cache trước
                var cachedAnswers = await _redisService.GetAsync<List<GetAllAssessmentAnswerByAttemptIdResponse>>(cacheKey);
                if (cachedAnswers != null)
                {
                    return ObjectResponse<List<GetAllAssessmentAnswerByAttemptIdResponse>>.SuccessResponse(cachedAnswers);
                }
                // 2. Nếu không có cache, query từ database
                var assessmentAnswers = await _unitOfWork.AssessmentAnswerRepository.GetAllByAsync(x => x.AttemptsId == query.Id);
                var answers = _mapper.Map<List<GetAllAssessmentAnswerByAttemptIdResponse>>(assessmentAnswers);
                
                // 3. Lưu vào cache
                await _redisService.SetAsync(cacheKey, answers, CacheExpiry);
                return ObjectResponse<List<GetAllAssessmentAnswerByAttemptIdResponse>>.SuccessResponse(answers);
            }
            catch (Exception e)
            {
                return ObjectResponse<List<GetAllAssessmentAnswerByAttemptIdResponse>>.FailureResponse(e);
            }
        }
    }
}
