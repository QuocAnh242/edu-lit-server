using Asp.Versioning;
using LessonServiceQuery.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using LessonServiceQuery.Application.Features.Sessions.GetSessionById;
using LessonServiceQuery.Application.Features.Sessions.GetSessionsByCourseId;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LessonServiceQuery.Api.Controllers;
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class SessionsController : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    public SessionsController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "ADMIN,TEACHER,STUDENT")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.Query(new GetSessionByIdQuery(id), cancellationToken);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }
    [HttpGet("by-course/{courseId:guid}")]
    [Authorize(Roles = "ADMIN,TEACHER,STUDENT")]
    public async Task<IActionResult> GetByCourseId(Guid courseId, CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.Query(new GetSessionsByCourseIdQuery(courseId), cancellationToken);
        return Ok(result);
    }
}