using Asp.Versioning;
using LessonServiceQuery.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using LessonServiceQuery.Application.Features.Courses.GetAllCourses;
using LessonServiceQuery.Application.Features.Courses.GetCourseById;
using LessonServiceQuery.Application.Features.Courses.GetCourseBySyllabusId;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LessonServiceQuery.Api.Controllers;
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class CoursesController : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    public CoursesController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }
    [HttpGet]
    [Authorize(Roles = "ADMIN,TEACHER,STUDENT")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.Query(new GetAllCoursesQuery(), cancellationToken);
        return Ok(result);
    }
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "ADMIN,TEACHER,STUDENT")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.Query(new GetCourseByIdQuery(id), cancellationToken);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }
    [HttpGet("by-syllabus/{syllabusId:guid}")]
    [Authorize(Roles = "ADMIN,TEACHER,STUDENT")]
    public async Task<IActionResult> GetBySyllabusId(Guid syllabusId, CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.Query(new GetCoursesBySyllabusIdQuery(syllabusId), cancellationToken);
        return Ok(result);
    }
}