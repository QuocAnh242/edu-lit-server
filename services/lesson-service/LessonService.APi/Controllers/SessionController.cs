using Asp.Versioning;
using LessonService.Api.Requests.Sessions;
using LessonService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using LessonService.Application.Features.Sessions.CreateSession;
using LessonService.Application.Features.Sessions.DeleteSession;
using LessonService.Application.Features.Sessions.UpdateSession;
using LessonService.Domain.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LessonService.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class SessionController : ControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public SessionController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateSession(CreateSessionRequest request)
    {
        var command = new CreateSessionCommand
        {
            CourseId = request.CourseId,
            Title = request.Title,
            Description = request.Description,
            Position = request.Position,
            DurationMinutes = request.DurationMinutes
        };

        var result = await _dispatcher.Send<CreateSessionCommand, Guid>(command, CancellationToken.None);
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
    public async Task<ActionResult<ApiResponse<object>>> UpdateSession(Guid id, UpdateSessionRequest request)
    {
        var command = new UpdateSessionCommand
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            Position = request.Position,
            DurationMinutes = request.DurationMinutes
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
    public async Task<ActionResult<ApiResponse<object>>> DeleteSession(Guid id)
    {
        var result = await _dispatcher.Send(new DeleteSessionCommand(id), CancellationToken.None);

        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }
}

