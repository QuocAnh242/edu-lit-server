using AutoMapper;
using QuestionService.Application.DTOs;
using QuestionService.Application.Features.QuestionOption.CreateQuestionOption;
using QuestionService.Domain.Entities;

namespace QuestionService.Application.Mappers
{
    public class QuestionOptionProfile : Profile
    {
        public QuestionOptionProfile()
        {
            CreateMap<CreateQuestionOptionCommand, QuestionOption>();
            CreateMap<QuestionOption, QuestionOptionDto>();
        }
    }
}



