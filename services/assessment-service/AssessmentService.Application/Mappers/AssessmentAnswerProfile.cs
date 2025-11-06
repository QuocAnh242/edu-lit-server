using AssessmentService.Application.Features.AssessmentAnswer.CreateAssessmentAnswer;
using AssessmentService.Application.Features.AssessmentAnswer.GetAllAssessmentAnswer;
using AssessmentService.Application.Features.AssessmentAnswer.GetAllAssessmentAnswerByAttemptId;
using AssessmentService.Application.Features.AssessmentAnswer.GetAssessmentAnswerById;
using AssessmentService.Application.Features.AssessmentAnswer.UpdateAssessmentAnswer;
using AssessmentService.Domain.Entities;
using AutoMapper;

namespace AssessmentService.Application.Mappers
{
    public class AssessmentAnswerProfile : Profile
    {
        public AssessmentAnswerProfile()
        {
            CreateMap<CreateAssessmentAnswerCommand, AssessmentAnswer>()
                .ForMember(dest => dest.AnswerId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<UpdateAssessmentAnswerCommand, AssessmentAnswer>()
                .ForMember(dest => dest.AnswerId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            CreateMap<AssessmentAnswer, GetAllAssessmentAnswerResponse>();

            CreateMap<AssessmentAnswer, GetAssessmentAnswerByIdResponse>();

            CreateMap<AssessmentAnswer, GetAllAssessmentAnswerByAttemptIdResponse>();
        }
    }
}
