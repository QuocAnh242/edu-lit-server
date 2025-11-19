using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.Features.Assessment.CreateAssessment;
using AssessmentService.Application.Features.Assessment.DeleteAssessment;
using AssessmentService.Application.Features.Assessment.GetAllAssessment;
using AssessmentService.Application.Features.Assessment.GetAssessmentById;
using AssessmentService.Application.Features.Assessment.UpdateAssessment;
using AssessmentService.Domain.Commons;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssessmentService.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = "TEACHER,ADMIN")]
    [Route("api/v1/[controller]")]
    public class AssessmentController : ControllerBase
    {
        private readonly ICommandHandler<CreateAssessmentCommand, int> _createAssessmentCommandHandler;
        private readonly ICommandHandler<UpdateAssessmentCommand, bool> _updateAssessmentCommandHandler;
        private readonly IQueryHandler<GetAssessmentByIdQuery, GetAssessmentByIdResponse> _getAssessmentByIdQueryHandler;
        private readonly IQueryHandler<GetAllAssessmentQuery, List<GetAllAssessmentResponse>> _getAllAssessmentQueryHandler;
        private readonly ICommandHandler<DeleteAssessmentCommand, bool> _deleteAssessmentCommandHandler;

        public AssessmentController(ICommandHandler<CreateAssessmentCommand, int> createAssessmentCommandHandler, ICommandHandler<UpdateAssessmentCommand, bool> updateAssessmentCommandHandler, IQueryHandler<GetAssessmentByIdQuery, GetAssessmentByIdResponse> getAssessmentByIdQueryHandler, IQueryHandler<GetAllAssessmentQuery, List<GetAllAssessmentResponse>> getAllAssessmentQueryHandler, ICommandHandler<DeleteAssessmentCommand, bool> deleteAssessmentCommandHandler)
        {
            _createAssessmentCommandHandler = createAssessmentCommandHandler;
            _updateAssessmentCommandHandler = updateAssessmentCommandHandler;
            _getAssessmentByIdQueryHandler = getAssessmentByIdQueryHandler;
            _getAllAssessmentQueryHandler = getAllAssessmentQueryHandler;
            _deleteAssessmentCommandHandler = deleteAssessmentCommandHandler;
        }

        // Stub GET action so CreatedAtAction has a target. Implement retrieval logic later.
        [HttpGet("{id}")]
        public async Task<ActionResult<ObjectResponse<GetAssessmentByIdResponse>>> GetAssessmentById(int id)
        {
            var result = await _getAssessmentByIdQueryHandler.Handle(new GetAssessmentByIdQuery(id), CancellationToken.None);
            if (result.Data is null)
            {
                if (result.ErrorCode == "404")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ObjectResponse<int>>> CreateAssessment([FromBody] CreateAssessmentCommand command)
        {
            var result = await _createAssessmentCommandHandler.Handle(command, CancellationToken.None);
            return CreatedAtAction(nameof(GetAssessmentById), new { id = result.Data }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ObjectResponse<bool>>> UpdateAssessment(int id, [FromBody] UpdateAssessmentCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest(ObjectResponse<bool>.Response("400", "ID in URL does not match ID in body", false));
            }
            var result = await _updateAssessmentCommandHandler.Handle(command, CancellationToken.None);
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ObjectResponse<List<GetAllAssessmentResponse>>>> GetAllAssessments()
        {
            var result = await _getAllAssessmentQueryHandler.Handle(new GetAllAssessmentQuery(), CancellationToken.None);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ObjectResponse<bool>>> DeleteAssessment(int id)
        {
            var result = await _deleteAssessmentCommandHandler.Handle(new DeleteAssessmentCommand(id), CancellationToken.None);
            return Ok(result);
        }

        [HttpGet]
        [Route("health")]
        public IActionResult HealthCheck()
        {
            return Ok("Assessment Service is healthy.");
        }
    }
}
