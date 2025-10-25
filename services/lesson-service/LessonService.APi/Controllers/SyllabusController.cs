using Asp.Versioning;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.Features.Syllabus.CreateSyllabus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LessonService.Application.Features.Syllabus.GetSyllabusById;
using LessonService.Domain.Commons;

namespace LessonService.APi.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
// [Authorize(Roles = "Admin,Teacher")]
public class SyllabusController : ControllerBase
{
    private readonly ICommandHandler<CreateSyllabusCommand, Guid> _createSyllabusCommandHandler;
    private readonly IQueryHandler<GetSyllabusByIdQuery, GetSyllabusByIdResponse> _getSyllabusByIdQueryHandler;
    
    public SyllabusController(ICommandHandler<CreateSyllabusCommand, Guid> createSyllabusCommandHandler, IQueryHandler<GetSyllabusByIdQuery, GetSyllabusByIdResponse> getSyllabusByIdQueryHandler)
    {
        _createSyllabusCommandHandler = createSyllabusCommandHandler;
        _getSyllabusByIdQueryHandler = getSyllabusByIdQueryHandler;
    }
    
    // Stub GET action so CreatedAtAction has a target. Implement retrieval logic later.
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<GetSyllabusByIdResponse>>> GetSyllabusById(Guid id)
    {
        var result = await _getSyllabusByIdQueryHandler.Handle(new GetSyllabusByIdQuery(id), CancellationToken.None);
        if (!result.Success)
        {
            if (result.StatusCode == 404)
                return NotFound(result);
            return BadRequest(result);
        }
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateSyllabus(CreateSyllabusCommand syllabusCommand)
    {
        // Safely read the user's id from claims before handling the command
        // var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // if (string.IsNullOrWhiteSpace(userIdString))
        //     return Unauthorized();
    
        // If your command expects a string author id, set it here.
        // syllabusCommand.Author = userIdString;
        syllabusCommand.Author = "Thai";
    
        var result = await _createSyllabusCommandHandler.Handle(syllabusCommand, CancellationToken.None);
        if (!result.Success)
        {
            if (result.StatusCode == 400)
                return UnprocessableEntity(result.Message);
            return BadRequest(result.Message);
        }

        return CreatedAtAction(nameof(GetSyllabusById), new { id = result.Data }, result);
    }


}