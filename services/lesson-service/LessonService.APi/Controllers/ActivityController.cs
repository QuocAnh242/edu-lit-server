using Asp.Versioning;
using LessonService.Api.Requests.Activities;
using LessonService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using LessonService.Application.Features.Activities.CreateActivity;
using LessonService.Application.Features.Activities.DeleteActivity;
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
    private readonly ICommandDispatcher _dispatcher;

    public ActivityController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
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

        var result = await _dispatcher.Send<CreateActivityCommand, Guid>(command, CancellationToken.None);
        if (!result.Success)
        {
            if (result.ErrorCode == 400)
                return UnprocessableEntity(result);
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateActivity(Guid id, UpdateActivityRequest request)
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

        var result = await _dispatcher.Send(command, CancellationToken.None);

        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteActivity(Guid id)
    {
        var result = await _dispatcher.Send(new DeleteActivityCommand(id), CancellationToken.None);

        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }
}

