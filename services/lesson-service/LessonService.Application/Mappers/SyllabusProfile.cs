using AutoMapper;
using LessonService.Application.Features.Syllabus.CreateSyllabus;
using LessonService.Application.Features.Syllabus.GetSyllabusById;
using LessonService.Application.Features.Syllabus.GetPagination;
using LessonService.Application.Features.Syllabus.GetPaginationSyllabus;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;

namespace LessonService.Application.Mappers;

public class SyllabusProfile : Profile
{
    public SyllabusProfile()
    {
        CreateMap<CreateSyllabusCommand, Syllabus>()
            .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Author)); 
        CreateMap<Syllabus, GetSyllabusByIdResponse>();
        CreateMap<Syllabus, GetSyllabusesResponse>();
        CreateMap<PagedResult<Syllabus>, PagedResult<GetSyllabusesResponse>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
    }
    
}