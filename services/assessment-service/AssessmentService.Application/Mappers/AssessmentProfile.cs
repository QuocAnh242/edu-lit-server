using AssessmentService.Application.Features.Assessment.CreateAssessment;
using AssessmentService.Application.Features.Assessment.GetAllAssessment;
using AssessmentService.Application.Features.Assessment.GetAssessmentById;
using AssessmentService.Application.Features.Assessment.UpdateAssessment;
using AssessmentService.Domain.Entities;
using AutoMapper;

namespace AssessmentService.Application.Mappers
{
    public class AssessmentProfile : Profile
    {
        public AssessmentProfile()
        {
            CreateMap<CreateAssessmentCommand, Assessment>()
                .ForMember(dest => dest.AssessmentId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateAssessmentCommand, Assessment>()
                .ForMember(dest => dest.AssessmentId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<Assessment, GetAllAssessmentResponse>();

            CreateMap<Assessment, GetAssessmentByIdResponse>();
        }
    }
}
