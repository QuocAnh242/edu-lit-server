using Asp.Versioning;
using LessonService.Api.Requests.Sessions;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.Features.Sessions.CreateSession;
using LessonService.Application.Features.Sessions.DeleteSession;
using LessonService.Application.Features.Sessions.GetSessionById;
using LessonService.Application.Features.Sessions.GetPaginationSessions;
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
    private readonly ICommandHandler<CreateSessionCommand, Guid> _createSessionCommandHandler;
    private readonly IQueryHandler<GetSessionByIdQuery, GetSessionByIdResponse> _getSessionByIdQueryHandler;
    private readonly ICommandHandler<UpdateSessionCommand> _updateSessionCommandHandler;
    private readonly ICommandHandler<DeleteSessionCommand> _deleteSessionCommandHandler;
    private readonly IQueryHandler<GetSessionsQuery, PagedResult<GetSessionsResponse>> _getSessionsQueryHandler;

    public SessionController(
        ICommandHandler<CreateSessionCommand, Guid> createSessionCommandHandler,
        IQueryHandler<GetSessionByIdQuery, GetSessionByIdResponse> getSessionByIdQueryHandler,
        ICommandHandler<UpdateSessionCommand> updateSessionCommandHandler,
        ICommandHandler<DeleteSessionCommand> deleteSessionCommandHandler,
        IQueryHandler<GetSessionsQuery, PagedResult<GetSessionsResponse>> getSessionsQueryHandler)
    {
        _createSessionCommandHandler = createSessionCommandHandler;
        _getSessionByIdQueryHandler = getSessionByIdQueryHandler;
        _updateSessionCommandHandler = updateSessionCommandHandler;
        _deleteSessionCommandHandler = deleteSessionCommandHandler;
        _getSessionsQueryHandler = getSessionsQueryHandler;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<GetSessionsResponse>>>> GetAllSessions([FromQuery] GetPaginationSessionsRequest request)
    {
        var query = new GetSessionsQuery
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            SearchTerm = request.SearchTerm,
            CourseId = request.CourseId
        };

        var result = await _getSessionsQueryHandler.Handle(query, CancellationToken.None);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetSessionByIdResponse>>> GetSessionById(Guid id)
    {
        var result = await _getSessionByIdQueryHandler.Handle(new GetSessionByIdQuery(id), CancellationToken.None);
        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }
        return Ok(result);
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

        var result = await _createSessionCommandHandler.Handle(command, CancellationToken.None);
        if (!result.Success)
        {
            if (result.ErrorCode == 400)
                return UnprocessableEntity(result);
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetSessionById), new { id = result.Data }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse>> UpdateSession(Guid id, UpdateSessionRequest request)
    {
        var command = new UpdateSessionCommand
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            Position = request.Position,
            DurationMinutes = request.DurationMinutes
        };

        var result = await _updateSessionCommandHandler.Handle(command, CancellationToken.None);

        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteSession(Guid id)
    {
        var result = await _deleteSessionCommandHandler.Handle(new DeleteSessionCommand(id), CancellationToken.None);

        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }
}

