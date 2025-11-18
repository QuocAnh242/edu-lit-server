using Asp.Versioning;
using LessonServiceQuery.Application.Abstractions.Messaging.Dispatcher.Interfaces;
using LessonServiceQuery.Application.Features.Syllabuses.GetAllSyllabuses;
using LessonServiceQuery.Application.Features.Syllabuses.GetSyllabusById;
using LessonServiceQuery.Application.Features.Syllabuses.GetSyllabusesBySubject;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace LessonServiceQuery.Api.Controllers;
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class SyllabusesController : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    public SyllabusesController(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.Query(new GetSyllabusByIdQuery(id), cancellationToken);
        if (!result.Success)
            return NotFound(result);
        return Ok(result);
    }
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.Query(new GetAllSyllabusesQuery(), cancellationToken);
        return Ok(result);
    }
    [HttpGet("by-subject/{subject}")]
    public async Task<IActionResult> GetBySubject(string subject, CancellationToken cancellationToken)
    {
        var result = await _queryDispatcher.Query(new GetSyllabusesBySubjectQuery(subject), cancellationToken);
        return Ok(result);
    }
}