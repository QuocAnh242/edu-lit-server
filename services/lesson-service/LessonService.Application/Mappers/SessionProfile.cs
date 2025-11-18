using AutoMapper;
using LessonService.Application.Features.Sessions.CreateSession;
using LessonService.Domain.Entities;

namespace LessonService.Application.Mappers;

public class SessionProfile : Profile
{
    public SessionProfile()
    {
        CreateMap<CreateSessionCommand, Session>();
    }
}

