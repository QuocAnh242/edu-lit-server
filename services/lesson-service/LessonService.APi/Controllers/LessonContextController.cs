using Asp.Versioning;
using LessonService.Api.Requests.LessonContexts;
using LessonService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using LessonService.Application.Features.LessonContexts.CreateBulkLessonContexts;
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
[Authorize]
public class LessonContextController : ControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public LessonContextController(ICommandDispatcher dispatcher)
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

    [HttpPost("bulk")]
    public async Task<ActionResult<ApiResponse<List<Guid>>>> CreateBulkLessonContexts(CreateBulkLessonContextsRequest request)
    {
        var command = new CreateBulkLessonContextsCommand
        {
            SessionId = request.SessionId,
            LessonContexts = request.LessonContexts.Select(item => new LessonContextItem
            {
                ParentLessonId = item.ParentLessonId,
                LessonTitle = item.LessonTitle,
                LessonContent = item.LessonContent,
                Position = item.Position,
                Level = item.Level
            }).ToList()
        };

        var result = await _dispatcher.Send<CreateBulkLessonContextsCommand, List<Guid>>(command, CancellationToken.None);
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

