using AutoMapper;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;
using System.Linq.Expressions;
using LessonService.Application.Features.Syllabus.GetPaginationSyllabus;
using LessonService.Application.Features.Syllabus.GetPaginationSyllabus;

namespace LessonService.Application.Features.Syllabus.GetPagination;

public class GetSyllabusesQueryHandler : IQueryHandler<GetSyllabusesQuery, PagedResult<GetSyllabusesResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSyllabusesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<GetSyllabusesResponse>>> Handle(GetSyllabusesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            // Build filter expression
            Expression<Func<Domain.Entities.Syllabus, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(query.SearchTerm) || query.Semester.HasValue || query.IsActive.HasValue)
            {
                filter = s =>
                    (string.IsNullOrWhiteSpace(query.SearchTerm) ||
                     s.Title.Contains(query.SearchTerm) ||
                     (s.Description != null && s.Description.Contains(query.SearchTerm)) ||
                     s.AcademicYear.Contains(query.SearchTerm)) &&
                    (!query.Semester.HasValue || s.Semester == query.Semester.Value) &&
                    (!query.IsActive.HasValue || s.IsActive == query.IsActive.Value);
            }

            var pagedResult = await _unitOfWork.SyllabusRepository.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                filter);

            var response = _mapper.Map<PagedResult<GetSyllabusesResponse>>(pagedResult);
            
            
            return ApiResponse<PagedResult<GetSyllabusesResponse>>.SuccessResponse(response, "Get Syllabuses Successfully");
        }
        catch (Exception e)
        {
            return ApiResponse<PagedResult<GetSyllabusesResponse>>.FailureResponse(e.Message, 500);
        }
    }
}

