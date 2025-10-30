using AutoMapper;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;
using System.Linq.Expressions;

namespace LessonService.Application.Features.Activities.GetPaginationActivities;

public class GetActivitiesQueryHandler : IQueryHandler<GetActivitiesQuery, PagedResult<GetActivitiesResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetActivitiesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<GetActivitiesResponse>>> Handle(GetActivitiesQuery query, CancellationToken cancellationToken)
    {
        try
        {
            // Build filter expression
            Expression<Func<Domain.Entities.Activity, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(query.SearchTerm) || query.SessionId.HasValue || !string.IsNullOrWhiteSpace(query.ActivityType))
            {
                filter = a =>
                    (string.IsNullOrWhiteSpace(query.SearchTerm) ||
                     a.Title.Contains(query.SearchTerm) ||
                     (a.Description != null && a.Description.Contains(query.SearchTerm))) &&
                    (!query.SessionId.HasValue || a.SessionId == query.SessionId.Value) &&
                    (string.IsNullOrWhiteSpace(query.ActivityType) || a.ActivityType == query.ActivityType);
            }

            var pagedResult = await _unitOfWork.ActivityRepository.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                filter);

            var response = _mapper.Map<PagedResult<GetActivitiesResponse>>(pagedResult);

            return ApiResponse<PagedResult<GetActivitiesResponse>>.SuccessResponse(response, "Get Activities Successfully");
        }
        catch (Exception e)
        {
            return ApiResponse<PagedResult<GetActivitiesResponse>>.FailureResponse(e.Message, 500);
        }
    }
}


