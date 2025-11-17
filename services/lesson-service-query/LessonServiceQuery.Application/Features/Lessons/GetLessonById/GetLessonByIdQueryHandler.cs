using AutoMapper;
using LessonService.Domain.Commons;
using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
using LessonServiceQuery.Domain.Repositories;
namespace LessonServiceQuery.Application.Features.Lessons.GetLessonById;
public class GetLessonByIdQueryHandler : IQueryHandler<GetLessonByIdQuery, LessonDto>
{
    private readonly ILessonRepository _repository;
    private readonly IMapper _mapper;
    public GetLessonByIdQueryHandler(ILessonRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<ApiResponse<LessonDto>> Handle(GetLessonByIdQuery query, CancellationToken cancellationToken = default)
    {
        var lesson = await _repository.GetByIdAsync(query.LessonId);
        if (lesson == null)
            return ApiResponse<LessonDto>.FailureResponse("Lesson not found", 404);
        var dto = _mapper.Map<LessonDto>(lesson);
        return ApiResponse<LessonDto>.SuccessResponse(dto);
    }
}