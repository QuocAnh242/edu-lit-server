using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.AssignmentAttempt.GetAllAssigmentAttempt
{
    public class GetAllAssignmentAttemptQueryHandler : IQueryHandler<GetAllAssignmentAttemptQuery, List<GetAllAssignmentAttemptResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assignmentAttempts:all";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(10);

        public GetAllAssignmentAttemptQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<List<GetAllAssignmentAttemptResponse>>> Handle(GetAllAssignmentAttemptQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Kiểm tra cache trước
                var cachedAssignmentAttempts = await _redisService.GetAsync<List<GetAllAssignmentAttemptResponse>>(CacheKey);
                if (cachedAssignmentAttempts != null)
                {
                    return ObjectResponse<List<GetAllAssignmentAttemptResponse>>.SuccessResponse(cachedAssignmentAttempts);
                }

                //2. Nếu không có cache, query từ database
                var assignmentAttemptEntities = await _unitOfWork.AssignmentAttemptRepository.GetAllAsync();
                var assignmentAttempts = _mapper.Map<List<GetAllAssignmentAttemptResponse>>(assignmentAttemptEntities);
                
                // 3. Lưu vào cache
                await _redisService.SetAsync(CacheKey, assignmentAttempts, CacheExpiry);

                return ObjectResponse<List<GetAllAssignmentAttemptResponse>>.SuccessResponse(assignmentAttempts);
            }
            catch (Exception e)
            {
                return ObjectResponse<List<GetAllAssignmentAttemptResponse>>.FailureResponse(e);
            }
        }
    }
}
