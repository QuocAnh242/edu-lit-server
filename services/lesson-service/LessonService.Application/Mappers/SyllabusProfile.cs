using AutoMapper;
using LessonService.Application.Features.Syllabus.CreateSyllabus;
using LessonService.Domain.Entities;

namespace LessonService.Application.Mappers;

public class SyllabusProfile : Profile
{
    public SyllabusProfile()
    {
        CreateMap<CreateSyllabusCommand, Syllabus>()
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Author)); }
    
}