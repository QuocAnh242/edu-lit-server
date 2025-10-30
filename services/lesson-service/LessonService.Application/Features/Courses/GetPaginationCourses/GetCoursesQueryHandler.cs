using AutoMapper;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;
using System.Linq.Expressions;

namespace LessonService.Application.Features.Courses.GetPaginationCourses;

public class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, PagedResult<GetCoursesResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetCoursesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<GetCoursesResponse>>> Handle(GetCoursesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            // Build filter expression
            Expression<Func<Domain.Entities.Course, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(query.SearchTerm) || query.SyllabusId.HasValue)
            {
                filter = c =>
                    (string.IsNullOrWhiteSpace(query.SearchTerm) ||
                     c.Title.Contains(query.SearchTerm) ||
                     c.CourseCode.Contains(query.SearchTerm) ||
                     (c.Description != null && c.Description.Contains(query.SearchTerm))) &&
                    (!query.SyllabusId.HasValue || c.SyllabusId == query.SyllabusId.Value);
            }

            var pagedResult = await _unitOfWork.CourseRepository.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                filter);

            var response = _mapper.Map<PagedResult<GetCoursesResponse>>(pagedResult);

            return ApiResponse<PagedResult<GetCoursesResponse>>.SuccessResponse(response, "Get Courses Successfully");
        }
        catch (Exception e)
        {
            return ApiResponse<PagedResult<GetCoursesResponse>>.FailureResponse(e.Message, 500);
        }
    }
}


