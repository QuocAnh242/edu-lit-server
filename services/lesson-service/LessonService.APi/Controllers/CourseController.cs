using Asp.Versioning;
using LessonService.Api.Requests.Courses;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.Features.Courses.CreateCourse;
using LessonService.Application.Features.Courses.DeleteCourse;
using LessonService.Application.Features.Courses.GetCourseById;
using LessonService.Application.Features.Courses.GetPaginationCourses;
using LessonService.Application.Features.Courses.UpdateCourse;
using LessonService.Domain.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LessonService.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class CourseController : ControllerBase
{
    private readonly ICommandHandler<CreateCourseCommand, Guid> _createCourseCommandHandler;
    private readonly IQueryHandler<GetCourseByIdQuery, GetCourseByIdResponse> _getCourseByIdQueryHandler;
    private readonly ICommandHandler<UpdateCourseCommand> _updateCourseCommandHandler;
    private readonly ICommandHandler<DeleteCourseCommand> _deleteCourseCommandHandler;
    private readonly IQueryHandler<GetCoursesQuery, PagedResult<GetCoursesResponse>> _getCoursesQueryHandler;

    public CourseController(
        ICommandHandler<CreateCourseCommand, Guid> createCourseCommandHandler,
        IQueryHandler<GetCourseByIdQuery, GetCourseByIdResponse> getCourseByIdQueryHandler,
        ICommandHandler<UpdateCourseCommand> updateCourseCommandHandler,
        ICommandHandler<DeleteCourseCommand> deleteCourseCommandHandler,
        IQueryHandler<GetCoursesQuery, PagedResult<GetCoursesResponse>> getCoursesQueryHandler)
    {
        _createCourseCommandHandler = createCourseCommandHandler;
        _getCourseByIdQueryHandler = getCourseByIdQueryHandler;
        _updateCourseCommandHandler = updateCourseCommandHandler;
        _deleteCourseCommandHandler = deleteCourseCommandHandler;
        _getCoursesQueryHandler = getCoursesQueryHandler;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<GetCoursesResponse>>>> GetAllCourses([FromQuery] GetPaginationCoursesRequest request)
    {
        var query = new GetCoursesQuery
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            SearchTerm = request.SearchTerm,
            SyllabusId = request.SyllabusId
        };

        var result = await _getCoursesQueryHandler.Handle(query, CancellationToken.None);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetCourseByIdResponse>>> GetCourseById(Guid id)
    {
        var result = await _getCourseByIdQueryHandler.Handle(new GetCourseByIdQuery(id), CancellationToken.None);
        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }
        return Ok(result);
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

        var result = await _createCourseCommandHandler.Handle(command, CancellationToken.None);
        if (!result.Success)
        {
            if (result.ErrorCode == 400)
                return UnprocessableEntity(result);
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetCourseById), new { id = result.Data }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse>> UpdateCourse(Guid id, UpdateCourseRequest request)
    {
        var command = new UpdateCourseCommand
        {
            Id = id,
            CourseCode = request.CourseCode,
            Title = request.Title,
            Description = request.Description
        };

        var result = await _updateCourseCommandHandler.Handle(command, CancellationToken.None);

        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteCourse(Guid id)
    {
        var result = await _deleteCourseCommandHandler.Handle(new DeleteCourseCommand(id), CancellationToken.None);

        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }

        return Ok(result);
    }
}

