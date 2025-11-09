using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.AssignmentAttempt.GetAssignmentAttemptById
{
    public class GetAssignmentAttemptByIdQueryHandler : IQueryHandler<GetAssignmentAttemptByIdQuery, GetAssignmentAttemptByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;

        public GetAssignmentAttemptByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<GetAssignmentAttemptByIdResponse>> Handle(GetAssignmentAttemptByIdQuery query, CancellationToken cancellationToken)
        {
            // Using redis to save cache for now
            var cacheKey = $"assignmentAttempt:{query.Id}";
            var cachedAssignmentAttempt = await _redisService.GetAsync<GetAssignmentAttemptByIdResponse>(cacheKey);
            if (cachedAssignmentAttempt is not null)
            {
                return ObjectResponse<GetAssignmentAttemptByIdResponse>.SuccessResponse(cachedAssignmentAttempt);
            }

            var assignAttemptEntity = await _unitOfWork.AssignmentAttemptRepository.GetByIdAsync(query.Id);
            if (assignAttemptEntity is null)
            {
                return ObjectResponse<GetAssignmentAttemptByIdResponse>.Response("404", "Assignment Attempt Not Found", null);
            }
            var assignmentAttempt = _mapper.Map<GetAssignmentAttemptByIdResponse>(assignAttemptEntity);

            // set redis cache for now
            await _redisService.SetAsync(cacheKey, assignmentAttempt, TimeSpan.FromMinutes(10));

            return ObjectResponse<GetAssignmentAttemptByIdResponse>.SuccessResponse(assignmentAttempt);
        }
    }
}
