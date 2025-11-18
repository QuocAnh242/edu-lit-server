using AutoMapper;
using LessonService.Domain.Commons;
using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
using LessonServiceQuery.Domain.Repositories;
namespace LessonServiceQuery.Application.Features.Lessons.GetAllLessons;
public class GetAllLessonsQueryHandler : IQueryHandler<GetAllLessonsQuery, List<LessonDto>>
{
    private readonly ILessonRepository _repository;
    private readonly IMapper _mapper;
    public GetAllLessonsQueryHandler(ILessonRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<ApiResponse<List<LessonDto>>> Handle(GetAllLessonsQuery query, CancellationToken cancellationToken = default)
    {
        var lessons = await _repository.GetAllAsync();
        var dtos = _mapper.Map<List<LessonDto>>(lessons);
        return ApiResponse<List<LessonDto>>.SuccessResponse(dtos);
    }
}