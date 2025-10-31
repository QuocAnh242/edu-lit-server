using AutoMapper;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Domain.Commons;
using LessonService.Domain.Interfaces;
using System.Linq.Expressions;

namespace LessonService.Application.Features.LessonContexts.GetPaginationLessonContexts;

public class GetLessonContextsQueryHandler : IQueryHandler<GetLessonContextsQuery, PagedResult<GetLessonContextsResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public GetLessonContextsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedResult<GetLessonContextsResponse>>> Handle(GetLessonContextsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            // Build filter expression
            Expression<Func<Domain.Entities.LessonContext, bool>>? filter = null;

            if (!string.IsNullOrWhiteSpace(query.SearchTerm) || query.SessionId.HasValue || query.ParentLessonId.HasValue)
            {
                filter = lc =>
                    (string.IsNullOrWhiteSpace(query.SearchTerm) ||
                     lc.LessonTitle.Contains(query.SearchTerm) ||
                     (lc.LessonContent != null && lc.LessonContent.Contains(query.SearchTerm))) &&
                    (!query.SessionId.HasValue || lc.SessionId == query.SessionId.Value) &&
                    (!query.ParentLessonId.HasValue || lc.ParentLessonId == query.ParentLessonId.Value);
            }

            var pagedResult = await _unitOfWork.LessonContextRepository.GetPagedAsync(
                query.PageNumber,
                query.PageSize,
                filter);

            var response = _mapper.Map<PagedResult<GetLessonContextsResponse>>(pagedResult);

            return ApiResponse<PagedResult<GetLessonContextsResponse>>.SuccessResponse(response, "Get LessonContexts Successfully");
        }
        catch (Exception e)
        {
            return ApiResponse<PagedResult<GetLessonContextsResponse>>.FailureResponse(e.Message, 500);
        }
    }
}


