using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Domain.Commons;
using AssessmentService.Application.IServices;
using AssessmentService.Domain.Interfaces;
using AutoMapper;

namespace AssessmentService.Application.Features.Assessment.GetAssessmentById
{
    public class GetAssessmentByIdQueryHandler : IQueryHandler<GetAssessmentByIdQuery, GetAssessmentByIdResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;

        public GetAssessmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }

        public async Task<ObjectResponse<GetAssessmentByIdResponse>> Handle(GetAssessmentByIdQuery query, CancellationToken cancellationToken)
        {
            var cacheKey = $"syllabus:{query.Id}";
            var cachedSyllabus = await _redisService.GetAsync<GetAssessmentByIdResponse>(cacheKey);
            if (cachedSyllabus is not null)
            {
                return ObjectResponse<GetAssessmentByIdResponse>.SuccessResponse(cachedSyllabus);
            }

            var syllabysEntity = await _unitOfWork.AssessmentRepository.GetByIdAsync(query.Id);
            if (syllabysEntity is null)
            {
                return ObjectResponse<GetAssessmentByIdResponse>.Response("404", "Syllabus Not Found", null);
            }
            var syllabus = _mapper.Map<GetAssessmentByIdResponse>(syllabysEntity);
            await _redisService.SetAsync(cacheKey, syllabus, TimeSpan.FromMinutes(10));
            return ObjectResponse<GetAssessmentByIdResponse>.SuccessResponse(syllabus);
        }

    }
}
