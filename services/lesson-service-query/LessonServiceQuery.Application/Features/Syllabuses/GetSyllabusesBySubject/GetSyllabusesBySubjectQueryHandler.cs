using AutoMapper;
using LessonService.Domain.Commons;
using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
using LessonServiceQuery.Domain.Repositories;
namespace LessonServiceQuery.Application.Features.Syllabuses.GetSyllabusesBySubject;
public class GetSyllabusesBySubjectQueryHandler : IQueryHandler<GetSyllabusesBySubjectQuery, List<SyllabusDto>>
{
    private readonly ISyllabusRepository _repository;
    private readonly IMapper _mapper;
    public GetSyllabusesBySubjectQueryHandler(ISyllabusRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<ApiResponse<List<SyllabusDto>>> Handle(GetSyllabusesBySubjectQuery query, CancellationToken cancellationToken = default)
    {
        var syllabuses = await _repository.GetBySubjectAsync(query.Subject);
        var dtos = _mapper.Map<List<SyllabusDto>>(syllabuses);
        return ApiResponse<List<SyllabusDto>>.SuccessResponse(dtos);
    }
}