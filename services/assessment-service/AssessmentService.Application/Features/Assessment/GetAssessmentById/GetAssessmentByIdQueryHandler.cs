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
        //private readonly IRedisService _redisService;

        /*public GetAssessmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper, IRedisService redisService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisService = redisService;
        }*/

        public GetAssessmentByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ObjectResponse<GetAssessmentByIdResponse>> Handle(GetAssessmentByIdQuery query, CancellationToken cancellationToken)
        {
            // not using redis to save cache for now
            /*var cacheKey = $"assessment:{query.Id}";
            var cachedAssessment = await _redisService.GetAsync<GetAssessmentByIdResponse>(cacheKey);
            if (cachedAssessment is not null)
            {
                return ObjectResponse<GetAssessmentByIdResponse>.SuccessResponse(cachedAssessment);
            }*/

            var assessmentEntity = await _unitOfWork.AssessmentRepository.GetByIdAsync(query.Id);
            if (assessmentEntity is null)
            {
                return ObjectResponse<GetAssessmentByIdResponse>.Response("404", "Assessment Not Found", null);
            }
            var assessment = _mapper.Map<GetAssessmentByIdResponse>(assessmentEntity);

            // not set redis cache for now
            //await _redisService.SetAsync(cacheKey, assessment, TimeSpan.FromMinutes(10));

            return ObjectResponse<GetAssessmentByIdResponse>.SuccessResponse(assessment);
        }

    }
}
