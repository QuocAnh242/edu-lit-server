using AssessmentService.Application.Features.GradingFeedback.CalculateGrading;
using AssessmentService.Application.Features.GradingFeedback.GetGradingFeedback;
using AssessmentService.Domain.Entities;
using AutoMapper;

namespace AssessmentService.Application.Mappers
{
    public class GradingFeedbackProfile : Profile
    {
        public GradingFeedbackProfile()
        {
            CreateMap<GradingFeedback, CalculateGradingResponse>();
            CreateMap<GradingFeedback, GetGradingFeedbackResponse>();
        }
    }
}
