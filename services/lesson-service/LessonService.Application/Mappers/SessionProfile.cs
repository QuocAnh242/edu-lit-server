using AutoMapper;
using LessonService.Application.Features.Sessions.CreateSession;
using LessonService.Application.Features.Sessions.GetSessionById;
using LessonService.Application.Features.Sessions.GetPaginationSessions;
using LessonService.Domain.Commons;
using LessonService.Domain.Entities;

namespace LessonService.Application.Mappers;

public class SessionProfile : Profile
{
    public SessionProfile()
    {
        CreateMap<CreateSessionCommand, Session>();
        CreateMap<Session, GetSessionByIdResponse>();
        CreateMap<Session, GetSessionsResponse>();
        CreateMap<PagedResult<Session>, PagedResult<GetSessionsResponse>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
    }
}

