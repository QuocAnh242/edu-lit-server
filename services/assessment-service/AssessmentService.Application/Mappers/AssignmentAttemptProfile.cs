using AssessmentService.Application.Features.AssignmentAttempt.CreateAssignmentAttempt;
using AssessmentService.Application.Features.AssignmentAttempt.GetAllAssigmentAttempt;
using AssessmentService.Application.Features.AssignmentAttempt.GetAllAssignmentAttemptByAssessmentId;
using AssessmentService.Application.Features.AssignmentAttempt.GetAssignmentAttemptById;
using AssessmentService.Application.Features.AssignmentAttempt.UpdateAssignmentAttempt;
using AssessmentService.Domain.Entities;
using AutoMapper;

namespace AssessmentService.Application.Mappers
{
    public class AssignmentAttemptProfile : Profile
    {
        public AssignmentAttemptProfile()
        {
            CreateMap<CreateAssignmentAttemptCommand, Domain.Entities.AssignmentAttempt>()
                .ForMember(dest => dest.AttemptsId, opt => opt.Ignore())
                .ForMember(dest => dest.StartedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateAssignmentAttemptCommand, Domain.Entities.AssignmentAttempt>()
                .ForMember(dest => dest.AttemptsId, opt => opt.Ignore())
                .ForMember(dest => dest.StartedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<AssignmentAttempt, GetAllAssignmentAttemptResponse>();
            CreateMap<AssignmentAttempt, GetAllAssignmentAttemptByAssessmentIdResponse>();
            CreateMap<AssignmentAttempt, GetAssignmentAttemptByIdResponse>();
        }
    }
}
