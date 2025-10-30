using Asp.Versioning;
using LessonService.Api.Requests.Activities;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.Features.Activities.CreateActivity;
using LessonService.Application.Features.Activities.DeleteActivity;
using LessonService.Application.Features.Activities.GetActivityById;
using LessonService.Application.Features.Activities.GetPaginationActivities;
using LessonService.Application.Features.Activities.UpdateActivity;
using LessonService.Domain.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LessonService.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class ActivityController : ControllerBase
{
    private readonly ICommandHandler<CreateActivityCommand, Guid> _createActivityCommandHandler;
    private readonly IQueryHandler<GetActivityByIdQuery, GetActivityByIdResponse> _getActivityByIdQueryHandler;
    private readonly ICommandHandler<UpdateActivityCommand> _updateActivityCommandHandler;
    private readonly ICommandHandler<DeleteActivityCommand> _deleteActivityCommandHandler;
    private readonly IQueryHandler<GetActivitiesQuery, PagedResult<GetActivitiesResponse>> _getActivitiesQueryHandler;

    public ActivityController(
        ICommandHandler<CreateActivityCommand, Guid> createActivityCommandHandler,
        IQueryHandler<GetActivityByIdQuery, GetActivityByIdResponse> getActivityByIdQueryHandler,
        ICommandHandler<UpdateActivityCommand> updateActivityCommandHandler,
        ICommandHandler<DeleteActivityCommand> deleteActivityCommandHandler,
        IQueryHandler<GetActivitiesQuery, PagedResult<GetActivitiesResponse>> getActivitiesQueryHandler)
    {
        _createActivityCommandHandler = createActivityCommandHandler;
        _getActivityByIdQueryHandler = getActivityByIdQueryHandler;
        _updateActivityCommandHandler = updateActivityCommandHandler;
        _deleteActivityCommandHandler = deleteActivityCommandHandler;
        _getActivitiesQueryHandler = getActivitiesQueryHandler;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<GetActivitiesResponse>>>> GetAllActivities([FromQuery] GetPaginationActivitiesRequest request)
    {
        var query = new GetActivitiesQuery
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            SearchTerm = request.SearchTerm,
            SessionId = request.SessionId,
            ActivityType = request.ActivityType
        };

        var result = await _getActivitiesQueryHandler.Handle(query, CancellationToken.None);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetActivityByIdResponse>>> GetActivityById(Guid id)
    {
        var result = await _getActivityByIdQueryHandler.Handle(new GetActivityByIdQuery(id), CancellationToken.None);
        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateActivity(CreateActivityRequest request)
    {
        var command = new CreateActivityCommand
        {
            SessionId = request.SessionId,
            Title = request.Title,
            Description = request.Description,
            ActivityType = request.ActivityType,
            Content = request.Content,
            Points = request.Points,
            Position = request.Position,
            IsRequired = request.IsRequired
        };

        var result = await _createActivityCommandHandler.Handle(command, CancellationToken.None);
        if (!result.Success)
        {
            if (result.ErrorCode == 400)
                return UnprocessableEntity(result);
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetActivityById), new { id = result.Data }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse>> UpdateActivity(Guid id, UpdateActivityRequest request)
    {
        var command = new UpdateActivityCommand
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            ActivityType = request.ActivityType,
            Content = request.Content,
            Points = request.Points,
            Position = request.Position,
            IsRequired = request.IsRequired
        };

        var result = await _updateActivityCommandHandler.Handle(command, CancellationToken.None);

        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteActivity(Guid id)
    {
        var result = await _deleteActivityCommandHandler.Handle(new DeleteActivityCommand(id), CancellationToken.None);

        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }
}


