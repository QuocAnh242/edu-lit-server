using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.Features.AssessmentAnswer.CreateAssessmentAnswer;
using AssessmentService.Application.Features.AssessmentAnswer.DeleteAssessmentAnswer;
using AssessmentService.Application.Features.AssessmentAnswer.GetAllAssessmentAnswer;
using AssessmentService.Application.Features.AssessmentAnswer.GetAllAssessmentAnswerByAttemptId;
using AssessmentService.Application.Features.AssessmentAnswer.GetAssessmentAnswerById;
using AssessmentService.Application.Features.AssessmentAnswer.UpdateAssessmentAnswer;
using Microsoft.AspNetCore.Mvc;

namespace AssessmentService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AssessmentAnswerController : ControllerBase
    {
        private readonly ICommandHandler<CreateAssessmentAnswerCommand, int> _create;
        private readonly ICommandHandler<UpdateAssessmentAnswerCommand, bool> _update;
        private readonly IQueryHandler<GetAssessmentAnswerByIdQuery, GetAssessmentAnswerByIdResponse> _getById;
        private readonly IQueryHandler<GetAllAssessmentAnswerQuery, List<GetAllAssessmentAnswerResponse>> _getAll;
        private readonly ICommandHandler<DeleteAssessmentAnswerCommand, bool> _delete;
        private readonly IQueryHandler<GetAllAssessmentAnswerByAttemptIdQuery, List<GetAllAssessmentAnswerByAttemptIdResponse>> _getAllByAttemptId;

        public AssessmentAnswerController(
            ICommandHandler<CreateAssessmentAnswerCommand, int> create,
            ICommandHandler<UpdateAssessmentAnswerCommand, bool> update,
            IQueryHandler<GetAssessmentAnswerByIdQuery, GetAssessmentAnswerByIdResponse> getById,
            IQueryHandler<GetAllAssessmentAnswerQuery, List<GetAllAssessmentAnswerResponse>> getAll,
            ICommandHandler<DeleteAssessmentAnswerCommand, bool> delete,
            IQueryHandler<GetAllAssessmentAnswerByAttemptIdQuery, List<GetAllAssessmentAnswerByAttemptIdResponse>> getAllByAttemptId)
        {
            _create = create;
            _update = update;
            _getById = getById;
            _getAll = getAll;
            _delete = delete;
            _getAllByAttemptId = getAllByAttemptId;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetAssessmentAnswerByIdResponse>> GetAssessmentAnswerById(int id)
        {
            var result = await _getById.Handle(new GetAssessmentAnswerByIdQuery(id), CancellationToken.None);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<List<GetAllAssessmentAnswerResponse>>> GetAllAssessmentAnswers()
        {
            var result = await _getAll.Handle(new GetAllAssessmentAnswerQuery(), CancellationToken.None);
            return Ok(result);
        }

        [HttpGet("attempt/{attemptId}")]
        public async Task<ActionResult<List<GetAllAssessmentAnswerByAttemptIdResponse>>> GetAllAssessmentAnswersByAttemptId(int attemptId)
        {
            var result = await _getAllByAttemptId.Handle(new GetAllAssessmentAnswerByAttemptIdQuery(attemptId), CancellationToken.None);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> CreateAssessmentAnswer([FromBody] CreateAssessmentAnswerCommand command)
        {
            var result = await _create.Handle(command, CancellationToken.None);
            return CreatedAtAction(nameof(GetAssessmentAnswerById), new { id = result }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> UpdateAssessmentAnswer(int id, [FromBody] UpdateAssessmentAnswerCommand command)
        {
            if (id != command.AnswerId)
            {
                return BadRequest("ID in URL does not match ID in body");
            }
            var result = await _update.Handle(command, CancellationToken.None);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteAssessmentAnswer(int id)
        {
            var result = await _delete.Handle(new DeleteAssessmentAnswerCommand(id), CancellationToken.None);
            return Ok(result);
        }

        [HttpGet]
        [Route("health")]
        public IActionResult HealthCheck()
        {
            return Ok("Assessment Question Service is healthy.");
        }
    }
}
