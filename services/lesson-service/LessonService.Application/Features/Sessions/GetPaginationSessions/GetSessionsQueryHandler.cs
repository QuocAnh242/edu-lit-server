using AutoMapper;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;
using System.Linq.Expressions;

namespace LessonService.Application.Features.Sessions.GetPaginationSessions;

public class GetSessionsQueryHandler : IQueryHandler<GetSessionsQuery, PagedResult<GetSessionsResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetSessionsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<GetSessionsResponse>>> Handle(GetSessionsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            // Build filter expression
            Expression<Func<Domain.Entities.Session, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(query.SearchTerm) || query.CourseId.HasValue)
            {
                filter = s =>
                    (string.IsNullOrWhiteSpace(query.SearchTerm) ||
                     s.Title.Contains(query.SearchTerm) ||
                     (s.Description != null && s.Description.Contains(query.SearchTerm))) &&
                    (!query.CourseId.HasValue || s.CourseId == query.CourseId.Value);
            }

            var pagedResult = await _unitOfWork.SessionRepository.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                filter);

            var response = _mapper.Map<PagedResult<GetSessionsResponse>>(pagedResult);

            return ApiResponse<PagedResult<GetSessionsResponse>>.SuccessResponse(response, "Get Sessions Successfully");
        }
        catch (Exception e)
        {
            return ApiResponse<PagedResult<GetSessionsResponse>>.FailureResponse(e.Message, 500);
        }
    }
}


