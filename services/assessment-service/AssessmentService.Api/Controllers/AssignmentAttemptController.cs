using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.Features.AssignmentAttempt.CreateAssignmentAttempt;
using AssessmentService.Application.Features.AssignmentAttempt.DeleteAssignmentAttempt;
using AssessmentService.Application.Features.AssignmentAttempt.GetAllAssigmentAttempt;
using AssessmentService.Application.Features.AssignmentAttempt.GetAllAssignmentAttemptByAssessmentId;
using AssessmentService.Application.Features.AssignmentAttempt.GetAssignmentAttemptByAssessmentId;
using AssessmentService.Application.Features.AssignmentAttempt.GetAssignmentAttemptById;
using AssessmentService.Application.Features.AssignmentAttempt.InviteUserToAssignmentAttempt;
using AssessmentService.Application.Features.AssignmentAttempt.UpdateAssignmentAttempt;
using AssessmentService.Domain.Commons;
using Microsoft.AspNetCore.Mvc;

namespace AssessmentService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AssignmentAttemptController : Controller
    {
        private readonly ICommandHandler<CreateAssignmentAttemptCommand, int> _create;
        private readonly ICommandHandler<UpdateAssignmentAttemptCommand, bool> _update;
        private readonly ICommandHandler<DeleteAssignmentAttemptCommand, bool> _delete;
        private readonly IQueryHandler<GetAssignmentAttemptByIdQuery, GetAssignmentAttemptByIdResponse> _getById;
        private readonly IQueryHandler<GetAllAssignmentAttemptQuery, List<GetAllAssignmentAttemptResponse>> _getAll;
        private readonly IQueryHandler<GetAllAssignmentAttemptByAssessmentIdQuery, List<GetAllAssignmentAttemptByAssessmentIdResponse>> _getAllByAssessmentId;
        private readonly ICommandHandler<InviteUserToAssignmentAttemptCommand, bool> _invite;

        public AssignmentAttemptController(
            ICommandHandler<CreateAssignmentAttemptCommand, int> create,
            ICommandHandler<UpdateAssignmentAttemptCommand, bool> update,
            ICommandHandler<DeleteAssignmentAttemptCommand, bool> delete,
            IQueryHandler<GetAssignmentAttemptByIdQuery, GetAssignmentAttemptByIdResponse> getById,
            IQueryHandler<GetAllAssignmentAttemptQuery, List<GetAllAssignmentAttemptResponse>> getAll,
            IQueryHandler<GetAllAssignmentAttemptByAssessmentIdQuery, List<GetAllAssignmentAttemptByAssessmentIdResponse>> getAllByAssessmentId,
            ICommandHandler<InviteUserToAssignmentAttemptCommand, bool> invite)
        {
            _create = create;
            _update = update;
            _delete = delete;
            _getById = getById;
            _getAll = getAll;
            _getAllByAssessmentId = getAllByAssessmentId;
            _invite = invite;

        }

        [HttpPost("invite")]
        public async Task<ActionResult> InviteUserToAssignmentAttempt([FromBody] InviteUserToAssignmentAttemptCommand command)
        {
            var result = await _invite.Handle(command, CancellationToken.None);
            if (result.Data)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ObjectResponse<GetAssignmentAttemptByIdResponse>>> GetAssignmentAttemptById(int id)
        {
            var result = await _getById.Handle(new GetAssignmentAttemptByIdQuery(id), CancellationToken.None);
            if (result.Data is null)
            {
                if (result.ErrorCode == "404")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ObjectResponse<List<GetAllAssignmentAttemptResponse>>>> GetAllAssignmentAttempts()
        {
            var result = await _getAll.Handle(new GetAllAssignmentAttemptQuery(), CancellationToken.None);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ObjectResponse<int>>> CreateAssignmentAttempt([FromBody] CreateAssignmentAttemptCommand command)
        {
            var result = await _create.Handle(command, CancellationToken.None);
            return CreatedAtAction(nameof(GetAssignmentAttemptById), new { id = result.Data }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ObjectResponse<bool>>> UpdateAssignmentAttempt(int id, [FromBody] UpdateAssignmentAttemptCommand command)
        {
            if (id != command.AttemptsId)
            {
                return BadRequest(ObjectResponse<bool>.Response("400", "ID in URL does not match ID in body", false));
            }
            var result = await _update.Handle(command, CancellationToken.None);
            if (result.Data)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ObjectResponse<bool>>> DeleteAssignmentAttempt(int id)
        {
            var result = await _delete.Handle(new DeleteAssignmentAttemptCommand(id), CancellationToken.None);
            if (result.Data)
            {
                return Ok(result);
            }
            return BadRequest(result);

        }

        [HttpGet]
        [Route("health")]
        public IActionResult HealthCheck()
        {
            return Ok("Assessment Question Service is healthy.");
        }
    }
}
