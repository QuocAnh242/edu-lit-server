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
            // ✅ Command → Entity mappings (Note: CreateAssessmentQuestionCommand now only has AssessmentId and QuestionId)
            CreateMap<CreateAssessmentQuestionCommand, AssessmentQuestion>()
                .ForMember(dest => dest.AssessmentQuestionId, opt => opt.Ignore())
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Assessment, opt => opt.Ignore())
                .ForMember(dest => dest.AssessmentAnswers, opt => opt.Ignore());

            // ✅ Update Command → Entity mappings
            CreateMap<UpdateAssessmentQuestionCommand, AssessmentQuestion>()
                .ForMember(dest => dest.QuestionId, opt => opt.MapFrom(src => src.QuestionId.ToString()))
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
