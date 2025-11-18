using AutoMapper;
using LessonService.Domain.Commons;
using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
using LessonServiceQuery.Domain.Repositories;
namespace LessonServiceQuery.Application.Features.Syllabuses.GetSyllabusById;
public class GetSyllabusByIdQueryHandler : IQueryHandler<GetSyllabusByIdQuery, SyllabusDto>
{
    private readonly ISyllabusRepository _repository;
    private readonly IMapper _mapper;
    public GetSyllabusByIdQueryHandler(ISyllabusRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<ApiResponse<SyllabusDto>> Handle(GetSyllabusByIdQuery query, CancellationToken cancellationToken = default)
    {
        var syllabus = await _repository.GetByIdAsync(query.SyllabusId);
        if (syllabus == null)
            return ApiResponse<SyllabusDto>.FailureResponse("Syllabus not found", 404);
        var dto = _mapper.Map<SyllabusDto>(syllabus);
        return ApiResponse<SyllabusDto>.SuccessResponse(dto);
    }
}