using Asp.Versioning;
using LessonService.Api.Requests.Courses;
using LessonService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using LessonService.Application.Features.Courses.CreateCourse;
using LessonService.Application.Features.Courses.DeleteCourse;
using LessonService.Application.Features.Courses.UpdateCourse;
using LessonService.Domain.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LessonService.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize(Roles = "ADMIN,TEACHER")]
public class CoursesController : ControllerBase
{
    private readonly ICommandDispatcher _dispatcher;

    public CoursesController(ICommandDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateCourse(CreateCourseRequest request)
    {
        var command = new CreateCourseCommand
        {
            SyllabusId = request.SyllabusId,
            CourseCode = request.CourseCode,
            Title = request.Title,
            Description = request.Description
        };

        var result = await _dispatcher.Send<CreateCourseCommand, Guid>(command, CancellationToken.None);
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
    public async Task<ActionResult<ApiResponse<object>>> UpdateCourse(Guid id, UpdateCourseRequest request)
    {
        var command = new UpdateCourseCommand
        {
            Id = id,
            CourseCode = request.CourseCode,
            Title = request.Title,
            Description = request.Description
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
    public async Task<ActionResult<ApiResponse<object>>> DeleteCourse(Guid id)
    {
        var result = await _dispatcher.Send(new DeleteCourseCommand(id), CancellationToken.None);

        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }
}

