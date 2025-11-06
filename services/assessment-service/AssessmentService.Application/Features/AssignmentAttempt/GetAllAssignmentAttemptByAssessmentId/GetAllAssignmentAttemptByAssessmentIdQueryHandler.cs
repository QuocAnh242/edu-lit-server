using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.Features.AssignmentAttempt.GetAssignmentAttemptByAssessmentId;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.AssignmentAttempt.GetAllAssignmentAttemptByAssessmentId
{
    public class GetAllAssignmentAttemptByAssessmentIdQueryHandler : IQueryHandler<GetAllAssignmentAttemptByAssessmentIdQuery, List<GetAllAssignmentAttemptByAssessmentIdResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(10);

        public GetAllAssignmentAttemptByAssessmentIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<List<GetAllAssignmentAttemptByAssessmentIdResponse>>>Handle(GetAllAssignmentAttemptByAssessmentIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                string cacheKey = $"assignmentAttempts:assessmentId:{query.Id}";

                // 1. Kiểm tra cache trước
                var cachedAttempts = await _redisService.GetAsync<List<GetAllAssignmentAttemptByAssessmentIdResponse>>(cacheKey);
                if (cachedAttempts != null)
                {
                    return ObjectResponse<List<GetAllAssignmentAttemptByAssessmentIdResponse>>.SuccessResponse(cachedAttempts);
                }

                // 2. Nếu không có cache, query từ database
                var assignmentAttempts = await _unitOfWork.AssignmentAttemptRepository.GetAllByAsync(x => x.AssessmentId == query.Id);
                var attempts = _mapper.Map<List<GetAllAssignmentAttemptByAssessmentIdResponse>>(assignmentAttempts);
                
                // 3. Lưu vào cache
                await _redisService.SetAsync(cacheKey, attempts, CacheExpiry);
                return ObjectResponse<List<GetAllAssignmentAttemptByAssessmentIdResponse>>.SuccessResponse(attempts);
            }
            catch (Exception e)
            {
                return ObjectResponse<List<GetAllAssignmentAttemptByAssessmentIdResponse>>.FailureResponse(e);
            }
        }
    }
}
