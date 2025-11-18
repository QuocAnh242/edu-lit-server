using System.Security.Claims;
using Asp.Versioning;
using LessonService.Api.Requests;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using LessonService.Application.Features.Syllabus.CreateSyllabus;
using Microsoft.AspNetCore.Mvc;
using LessonService.Application.Features.Syllabus.DeleteSyllabus;
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
    private readonly ICommandHandler<UpdateSyllabusCommand> _updateSyllabusCommandHandler;
    private readonly ICommandHandler<DeleteSyllabusCommand> _deleteSyllabusCommandHandler;
    private readonly ICommandDispatcher _dispatcher;
    
    public SyllabusController(
        ICommandHandler<CreateSyllabusCommand, Guid> createSyllabusCommandHandler,  
        ICommandHandler<UpdateSyllabusCommand> updateSyllabusCommandHandler,
        ICommandHandler<DeleteSyllabusCommand> deleteSyllabusCommandHandler,
        ICommandDispatcher dispatcher)
    {
        _createSyllabusCommandHandler = createSyllabusCommandHandler;
        _updateSyllabusCommandHandler = updateSyllabusCommandHandler;
        _deleteSyllabusCommandHandler = deleteSyllabusCommandHandler;
        _dispatcher = dispatcher;
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
    
        var result = await _dispatcher.Send<CreateSyllabusCommand, Guid>(command, CancellationToken.None);
        if (!result.Success)
        {
            if (result.ErrorCode == 400)
                return UnprocessableEntity(result);
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> UpdateSyllabus(Guid id, UpdateSyllabusRequest request)
    {
        var command = new UpdateSyllabusCommand()
        {
            Id = id,
            AcademicYear = request.AcademicYear,
            Semester = request.Semester,
            Title = request.Title,
            Description = request.Description,
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
    public async Task<ActionResult<ApiResponse<object>>> DeleteSyllabus(Guid id)
    {
        var result = await _dispatcher.Send(new DeleteSyllabusCommand(id), CancellationToken.None);
        
        if (!result.Success)
        {
            if (result.ErrorCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }
        
        return Ok(result);
    }
}