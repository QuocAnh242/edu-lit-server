using Asp.Versioning;
using LessonService.Application.Abstractions.Messaging;
using LessonService.Application.Features.Syllabus.CreateSyllabus;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LessonService.APi.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
// [Authorize(Roles = "Admin,Teacher")]
public class SyllabusController : ControllerBase
{
    private readonly ICommandHandler<CreateSyllabusCommand, Guid> _createSyllabusCommandHandler;
    
    public SyllabusController(ICommandHandler<CreateSyllabusCommand, Guid> createSyllabusCommandHandler)
    {
        _createSyllabusCommandHandler = createSyllabusCommandHandler;
    }
    
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateSyllabus(CreateSyllabusCommand syllabusCommand)
    {
        // Safely read the user's id from claims before handling the command
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userIdString))
            return Unauthorized();

        // If your command expects a string author id, set it here.
        syllabusCommand.Author = userIdString;

        var result = await _createSyllabusCommandHandler.Handle(syllabusCommand, CancellationToken.None);
        if (result.IsFailure)
        {
            if (result.Error.Code.Contains("Validation"))
                return UnprocessableEntity(result.Error);
            return BadRequest(result.Error);
        }
        
        // include the API version route value so URL generation works with api versioning
        var apiVersion = RouteData.Values["version"]?.ToString();

        return CreatedAtAction(
            nameof(GetSyllabusById), // action used to retrieve the created resource
            new { version = apiVersion, id = result.Value },  // route values (include version)
            result.Value);
    }

    // Stub GET action so CreatedAtAction has a target. Implement retrieval logic later.
    [HttpGet("{id}")]
    public async Task<ActionResult<Guid>> GetSyllabusById(Guid id)
    {
        // TODO: implement retrieval from database; return NotFound() if missing
        return NotFound();
    }
}