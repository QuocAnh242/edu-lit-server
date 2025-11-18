using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Commons;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.Assessment.GetAllAssessment
{
    public class GetAllAssessmentQueryHandler : IQueryHandler<GetAllAssessmentQuery, List<GetAllAssessmentResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;
        private const string CacheKey = "assessments:all";
        private static readonly TimeSpan CacheExpiry = TimeSpan.FromMinutes(10);

        public GetAllAssessmentQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<List<GetAllAssessmentResponse>>> Handle(GetAllAssessmentQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Kiểm tra cache trước
                var cachedAssessments = await _redisService.GetAsync<List<GetAllAssessmentResponse>>(CacheKey);

                if (cachedAssessments != null)
                {
                    return ObjectResponse<List<GetAllAssessmentResponse>>.SuccessResponse(cachedAssessments);
                }

                // 2. Nếu không có cache, query từ database
                var assessmentEntities = await _unitOfWork.AssessmentRepository.GetAllAsync();
                var assessments = _mapper.Map<List<GetAllAssessmentResponse>>(assessmentEntities);

                // 3. Lưu vào cache
                await _redisService.SetAsync(CacheKey, assessments, CacheExpiry);

                return ObjectResponse<List<GetAllAssessmentResponse>>.SuccessResponse(assessments);
            }
            catch (Exception e)
            {
                return ObjectResponse<List<GetAllAssessmentResponse>>.FailureResponse(e);
            }
        }
    }
}
