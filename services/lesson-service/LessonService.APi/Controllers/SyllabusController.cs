using System.Security.Claims;
using Asp.Versioning;
using LessonService.Api.Requests;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.Features.Syllabus.CreateSyllabus;
using Microsoft.AspNetCore.Mvc;
using LessonService.Application.Features.Syllabus.GetSyllabusById;
using LessonService.Application.Features.Syllabus.DeleteSyllabus;
using LessonService.Application.Features.Syllabus.GetPaginationSyllabus;
using LessonService.Application.Features.Syllabus.UpdateSyllabus;
using LessonService.Domain.Commons;
using Microsoft.AspNetCore.Authorization;

namespace LessonService.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
// [Authorize(Roles = "Admin,Teacher")]
[Authorize]
public class SyllabusController : ControllerBase
{
    private readonly ICommandHandler<CreateSyllabusCommand, Guid> _createSyllabusCommandHandler;
    private readonly IQueryHandler<GetSyllabusByIdQuery, GetSyllabusByIdResponse> _getSyllabusByIdQueryHandler;
    private readonly ICommandHandler<UpdateSyllabusCommand> _updateSyllabusCommandHandler;
    private readonly ICommandHandler<DeleteSyllabusCommand> _deleteSyllabusCommandHandler;
    private readonly IQueryHandler<GetSyllabusesQuery, PagedResult<GetSyllabusesResponse>> _getSyllabusesQueryHandler;
    
    public SyllabusController(
        ICommandHandler<CreateSyllabusCommand, Guid> createSyllabusCommandHandler,  
        IQueryHandler<GetSyllabusByIdQuery, GetSyllabusByIdResponse> getSyllabusByIdQueryHandler,   
        ICommandHandler<UpdateSyllabusCommand> updateSyllabusCommandHandler,
        ICommandHandler<DeleteSyllabusCommand> deleteSyllabusCommandHandler,
        IQueryHandler<GetSyllabusesQuery, PagedResult<GetSyllabusesResponse>> getSyllabusesQueryHandler)
    {
        _createSyllabusCommandHandler = createSyllabusCommandHandler;
        _getSyllabusByIdQueryHandler = getSyllabusByIdQueryHandler;
        _updateSyllabusCommandHandler = updateSyllabusCommandHandler;
        _deleteSyllabusCommandHandler = deleteSyllabusCommandHandler;
        _getSyllabusesQueryHandler = getSyllabusesQueryHandler;
    }
    
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<GetSyllabusesResponse>>>> GetAllSyllabuses([FromQuery] GetPaginationSyllabusRequest query)
    {
        var command = new GetSyllabusesQuery()
        {
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            SearchTerm = query.SearchTerm,
            Semester = query.Semester,
            IsActive = query.IsActive
        };
        
        var result = await _getSyllabusesQueryHandler.Handle(command, CancellationToken.None);
        if (!result.Success)
            return BadRequest(result);
        
        return Ok(result);
    }
    
    // Stub GET action so CreatedAtAction has a target. Implement retrieval logic later.
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetSyllabusByIdResponse>>> GetSyllabusById(Guid id)
    {
        var result = await _getSyllabusByIdQueryHandler.Handle(new GetSyllabusByIdQuery(id), CancellationToken.None);
        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateSyllabus(CreateSyllabusRequest request)
    {
        // Safely read the user's id from claims before handling the command
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdString))
            return Unauthorized();

        var command = new CreateSyllabusCommand()
        {
            AcademicYear = request.AcademicYear,
            Semester = request.Semester,
            Title = request.Title,
            Description = request.Description,
            Author = userIdString
        };
    
        var result = await _createSyllabusCommandHandler.Handle(command, CancellationToken.None);
        if (!result.Success)
        {
            if (result.ErrorCode == 400)
                return UnprocessableEntity(result);
            return BadRequest(result);
        }

        return CreatedAtAction(nameof(GetSyllabusById), new { id = result.Data }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse>> UpdateSyllabus(Guid id, UpdateSyllabusRequest request)
    {
        var command = new UpdateSyllabusCommand()
        {
            Id = id,
            AcademicYear = request.AcademicYear,
            Semester = request.Semester,
            Title = request.Title,
            Description = request.Description,
        };
        
        var result = await _updateSyllabusCommandHandler.Handle(command, CancellationToken.None);
        
        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }
        
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteSyllabus(Guid id)
    {
        var result = await _deleteSyllabusCommandHandler.Handle(new DeleteSyllabusCommand(id), CancellationToken.None);
        
        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }
        
        return Ok(result);
    }
}