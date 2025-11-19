using Asp.Versioning;
using LessonServiceQuery.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using LessonServiceQuery.Application.Features.Lessons.GetAllLessons;
using LessonServiceQuery.Application.Features.Lessons.GetLessonById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LessonServiceQuery.Api.Controllers;
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize(Roles = "ADMIN,TEACHER")]
public class LessonsController : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    public LessonsController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.Query(new GetLessonByIdQuery(id), cancellationToken);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.Query(new GetAllLessonsQuery(), cancellationToken);
        return Ok(result);
    }
}