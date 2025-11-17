using AutoMapper;
using LessonService.Domain.Commons;
using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
using LessonServiceQuery.Domain.Repositories;
namespace LessonServiceQuery.Application.Features.Sessions.GetSessionsByCourseId;
public class GetSessionsByCourseIdQueryHandler : IQueryHandler<GetSessionsByCourseIdQuery, List<SessionDto>>
{
    private readonly ISessionRepository _repository;
    private readonly IMapper _mapper;
    public GetSessionsByCourseIdQueryHandler(ISessionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<ApiResponse<List<SessionDto>>> Handle(GetSessionsByCourseIdQuery query, CancellationToken cancellationToken = default)
    {
        var sessions = await _repository.GetByCourseIdAsync(query.CourseId);
        var dtos = _mapper.Map<List<SessionDto>>(sessions);
        return ApiResponse<List<SessionDto>>.SuccessResponse(dtos);
    }
}