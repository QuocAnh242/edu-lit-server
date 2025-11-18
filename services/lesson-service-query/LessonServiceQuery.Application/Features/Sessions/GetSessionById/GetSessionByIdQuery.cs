using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
namespace LessonServiceQuery.Application.Features.Sessions.GetSessionById;
public record GetSessionByIdQuery(Guid SessionId) : IQuery<SessionDto>;