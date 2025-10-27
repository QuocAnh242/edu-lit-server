using AutoMapper;
using QuestionService.Application.DTOs;
using QuestionService.Application.Features.Question.CreateQuestion;
using QuestionService.Domain.Entities;

namespace QuestionService.Application.Mappers
{
    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<CreateQuestionCommand, Question>();
            CreateMap<Question, QuestionDto>();
        }
    }
}

