using AutoMapper;
using QuestionService.Application.DTOs;
using QuestionService.Application.Features.QuestionBank.CreateQuestionBank;
using QuestionService.Domain.Entities;

namespace QuestionService.Application.Mappers
{
    public class QuestionBankProfile : Profile
    {
        public QuestionBankProfile()
        {
            CreateMap<CreateQuestionBankCommand, QuestionBank>();
            CreateMap<QuestionBank, QuestionBankDto>()
                .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions));
        }
    }
}

