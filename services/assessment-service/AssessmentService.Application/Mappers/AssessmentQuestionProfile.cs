using AssessmentService.Application.Features.AssessmentQuestion.CreateAssessmentQuestion;
using AssessmentService.Application.Features.AssessmentQuestion.GetAllAssessmentQuestion;
using AssessmentService.Application.Features.AssessmentQuestion.GetAllAssessmentQuestionByAssessmentId;
using AssessmentService.Application.Features.AssessmentQuestion.GetAssessmentQuestionById;
using AssessmentService.Application.Features.AssessmentQuestion.UpdateAssessmentQuestion;
using AssessmentService.Domain.Entities;
using AutoMapper;

namespace AssessmentService.Application.Mappers
{
    public class AssessmentQuestionProfile : Profile
    {
        public AssessmentQuestionProfile()
        {
            // ✅ Command → Entity mappings
            CreateMap<CreateAssessmentQuestionCommand, AssessmentQuestion>()
                .ForMember(dest => dest.AssessmentQuestionId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Assessment, opt => opt.Ignore())
                .ForMember(dest => dest.AssessmentAnswers, opt => opt.Ignore());

            // ✅ FIX: Đổi từ UpdateAssessmentAnswerCommand → UpdateAssessmentQuestionCommand
            CreateMap<UpdateAssessmentQuestionCommand, AssessmentQuestion>()
                .ForMember(dest => dest.AssessmentQuestionId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Assessment, opt => opt.Ignore())
                .ForMember(dest => dest.AssessmentAnswers, opt => opt.Ignore());

            // ✅ Entity → Response mappings
            CreateMap<AssessmentQuestion, GetAllAssessmentQuestionResponse>();
            CreateMap<AssessmentQuestion, GetAllAssessmentQuestionByAssessmentIdResponse>();
            CreateMap<AssessmentQuestion, GetAssessmentQuestionByIdResponse>();
        }
    }
}
