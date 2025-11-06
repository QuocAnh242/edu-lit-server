using AssessmentService.Application.Features.AssessmentAnswer.UpdateAssessmentAnswer;
using AssessmentService.Application.Features.AssessmentQuestion.CreateAssessmentQuestion;
using AssessmentService.Application.Features.AssessmentQuestion.GetAllAssessmentQuestion;
using AssessmentService.Application.Features.AssessmentQuestion.GetAllAssessmentQuestionByAssessmentId;
using AssessmentService.Application.Features.AssessmentQuestion.GetAssessmentQuestionById;
using AssessmentService.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentService.Application.Mappers
{
    public class AssessmentQuestionProfile : Profile
    {
        public AssessmentQuestionProfile()
        {
            CreateMap<CreateAssessmentQuestionCommand, AssessmentQuestion>()
                .ForMember(dest => dest.AssessmentQuestionId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<UpdateAssessmentAnswerCommand, AssessmentQuestion>()
                .ForMember(dest => dest.AssessmentQuestionId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<AssessmentQuestion, GetAllAssessmentQuestionResponse>();
            CreateMap<AssessmentQuestion, GetAllAssessmentQuestionByAssessmentIdResponse>();
            CreateMap<AssessmentQuestion, GetAssessmentQuestionByIdResponse>();
        }
    }
}
