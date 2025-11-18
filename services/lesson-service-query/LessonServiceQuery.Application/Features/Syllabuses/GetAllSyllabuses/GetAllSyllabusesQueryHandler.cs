using AutoMapper;
using LessonService.Domain.Commons;
using LessonServiceQuery.Application.Abstractions.Messaging;
using LessonServiceQuery.Application.DTOs;
using LessonServiceQuery.Domain.Repositories;
namespace LessonServiceQuery.Application.Features.Syllabuses.GetAllSyllabuses;
public class GetAllSyllabusesQueryHandler : IQueryHandler<GetAllSyllabusesQuery, List<SyllabusDto>>
{
    private readonly ISyllabusRepository _repository;
    private readonly IMapper _mapper;
    public GetAllSyllabusesQueryHandler(ISyllabusRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    public async Task<ApiResponse<List<SyllabusDto>>> Handle(GetAllSyllabusesQuery query, CancellationToken cancellationToken = default)
    {
        var syllabuses = await _repository.GetAllAsync();
        var dtos = _mapper.Map<List<SyllabusDto>>(syllabuses);
        return ApiResponse<List<SyllabusDto>>.SuccessResponse(dtos);
    }
}