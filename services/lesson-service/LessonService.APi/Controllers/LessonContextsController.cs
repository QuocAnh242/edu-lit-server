using Asp.Versioning;
using LessonService.Api.Requests.LessonContexts;
using LessonService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using LessonService.Application.Features.LessonContexts.BulkUpdateLessonContext;
using LessonService.Application.Features.LessonContexts.CreateBulkLessonContext;
using LessonService.Application.Features.LessonContexts.CreateLessonContext;
using LessonService.Application.Features.LessonContexts.DeleteLessonContext;
using LessonService.Application.Features.LessonContexts.UpdateLessonContext;
using LessonService.Domain.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LessonService.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize(Roles = "ADMIN,TEACHER")]
public class LessonContextsController : ControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public LessonContextsController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
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

        var result = await _dispatcher.Send<CreateLessonContextCommand, Guid>(command, CancellationToken.None);
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
    
    /// <summary>
    /// Create multiple lesson contexts with automatic parent detection based on level and position.
    /// This is the recommended endpoint for document-style content creation.
    /// </summary>
    [HttpPost("bulk")]
    public async Task<IActionResult> CreateBulkLessonContextDocument(CreateBulkLessonContextRequest request)
    {
        var command = new CreateBulkLessonContextCommand(
            request.SessionId,
            request.LessonContexts.Select(x => new LessonContextItemDto
            {
                LessonTitle = x.LessonTitle,
                LessonContent = x.LessonContent,
                Position = x.Position,
                Level = x.Level
            }).ToList()
        );

        var result = await _dispatcher.Send<CreateBulkLessonContextCommand, CreateBulkLessonContextResponse>(command, CancellationToken.None);

        if (!result.Success)
        {
            return StatusCode(result.ErrorCode ?? 500, result);
        }

        return StatusCode(result.ErrorCode ?? 200, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateLessonContext(Guid id, UpdateLessonContextRequest request)
    {
        var command = new UpdateLessonContextCommand
        {
            Id = id,
            LessonTitle = request.LessonTitle,
            LessonContent = request.LessonContent,
            Position = request.Position,
            Level = request.Level
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

    /// <summary>
    /// Bulk update multiple lesson contexts at once.
    /// Supports partial updates - only provide fields you want to change.
    /// Useful for re-ordering, re-leveling, or batch content updates.
    /// </summary>
    [HttpPut("bulk-update")]
    public async Task<IActionResult> BulkUpdateLessonContexts(BulkUpdateLessonContextRequest request)
    {
        var command = new BulkUpdateLessonContextCommand(
            request.Items.Select(x => new LessonContextUpdateItemDto
            {
                Id = x.Id,
                LessonTitle = x.LessonTitle,
                LessonContent = x.LessonContent,
                Position = x.Position,
                Level = x.Level
            }).ToList()
        );

        var result = await _dispatcher.Send<BulkUpdateLessonContextCommand, BulkUpdateLessonContextResponse>(
            command, 
            CancellationToken.None
        );

        if (!result.Success)
        {
            return StatusCode(result.ErrorCode ?? 500, result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteLessonContext(Guid id)
    {
        var result = await _dispatcher.Send(new DeleteLessonContextCommand(id), CancellationToken.None);

        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }
}

