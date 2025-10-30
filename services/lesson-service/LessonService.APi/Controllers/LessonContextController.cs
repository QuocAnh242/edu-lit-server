using Asp.Versioning;
using LessonService.Api.Requests.LessonContexts;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.Features.LessonContexts.CreateLessonContext;
using LessonService.Application.Features.LessonContexts.DeleteLessonContext;
using LessonService.Application.Features.LessonContexts.GetLessonContextById;
using LessonService.Application.Features.LessonContexts.GetPaginationLessonContexts;
using LessonService.Application.Features.LessonContexts.UpdateLessonContext;
using LessonService.Domain.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LessonService.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class LessonContextController : ControllerBase
{
    private readonly ICommandHandler<CreateLessonContextCommand, Guid> _createLessonContextCommandHandler;
    private readonly IQueryHandler<GetLessonContextByIdQuery, GetLessonContextByIdResponse> _getLessonContextByIdQueryHandler;
    private readonly ICommandHandler<UpdateLessonContextCommand> _updateLessonContextCommandHandler;
    private readonly ICommandHandler<DeleteLessonContextCommand> _deleteLessonContextCommandHandler;
    private readonly IQueryHandler<GetLessonContextsQuery, PagedResult<GetLessonContextsResponse>> _getLessonContextsQueryHandler;

    public LessonContextController(
        ICommandHandler<CreateLessonContextCommand, Guid> createLessonContextCommandHandler,
        IQueryHandler<GetLessonContextByIdQuery, GetLessonContextByIdResponse> getLessonContextByIdQueryHandler,
        ICommandHandler<UpdateLessonContextCommand> updateLessonContextCommandHandler,
        ICommandHandler<DeleteLessonContextCommand> deleteLessonContextCommandHandler,
        IQueryHandler<GetLessonContextsQuery, PagedResult<GetLessonContextsResponse>> getLessonContextsQueryHandler)
    {
        _createLessonContextCommandHandler = createLessonContextCommandHandler;
        _getLessonContextByIdQueryHandler = getLessonContextByIdQueryHandler;
        _updateLessonContextCommandHandler = updateLessonContextCommandHandler;
        _deleteLessonContextCommandHandler = deleteLessonContextCommandHandler;
        _getLessonContextsQueryHandler = getLessonContextsQueryHandler;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<GetLessonContextsResponse>>>> GetAllLessonContexts([FromQuery] GetPaginationLessonContextsRequest request)
    {
        var query = new GetLessonContextsQuery
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            SearchTerm = request.SearchTerm,
            SessionId = request.SessionId,
            ParentLessonId = request.ParentLessonId
        };

        var result = await _getLessonContextsQueryHandler.Handle(query, CancellationToken.None);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetLessonContextByIdResponse>>> GetLessonContextById(Guid id)
    {
        var result = await _getLessonContextByIdQueryHandler.Handle(new GetLessonContextByIdQuery(id), CancellationToken.None);
        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateLessonContext(CreateLessonContextRequest request)
    {
        var command = new CreateLessonContextCommand
        {
            SessionId = request.SessionId,
            ParentLessonId = request.ParentLessonId,
            LessonTitle = request.LessonTitle,
            LessonContent = request.LessonContent,
            Position = request.Position,
            Level = request.Level
        };

        var result = await _createLessonContextCommandHandler.Handle(command, CancellationToken.None);
        if (!result.Success)
        {
            if (result.ErrorCode == 400)
                return UnprocessableEntity(result);
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetLessonContextById), new { id = result.Data }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse>> UpdateLessonContext(Guid id, UpdateLessonContextRequest request)
    {
        var command = new UpdateLessonContextCommand
        {
            Id = id,
            LessonTitle = request.LessonTitle,
            LessonContent = request.LessonContent,
            Position = request.Position,
            Level = request.Level
        };

        var result = await _updateLessonContextCommandHandler.Handle(command, CancellationToken.None);

        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteLessonContext(Guid id)
    {
        var result = await _deleteLessonContextCommandHandler.Handle(new DeleteLessonContextCommand(id), CancellationToken.None);

        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }
}


