using AutoMapper;
using LessonService.Domain.Commons;
using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
using LessonServiceQuery.Domain.Repositories;
namespace LessonServiceQuery.Application.Features.Sessions.GetSessionById;
public class GetSessionByIdQueryHandler : IQueryHandler<GetSessionByIdQuery, SessionDto>
{
    private readonly ISessionRepository _repository;
    private readonly IMapper _mapper;
    public GetSessionByIdQueryHandler(ISessionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<ApiResponse<SessionDto>> Handle(GetSessionByIdQuery query, CancellationToken cancellationToken = default)
    {
        var session = await _repository.GetByIdAsync(query.SessionId);
        if (session == null)
            return ApiResponse<SessionDto>.FailureResponse("Session not found", 404);
        var dto = _mapper.Map<SessionDto>(session);
        return ApiResponse<SessionDto>.SuccessResponse(dto);
    }
}