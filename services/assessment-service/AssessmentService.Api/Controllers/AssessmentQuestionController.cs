using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.Features.AssessmentQuestion.CreateAssessmentQuestion;
using AssessmentService.Application.Features.AssessmentQuestion.DeleteAssessmentQuestion;
using AssessmentService.Application.Features.AssessmentQuestion.GetAllAssessmentQuestion;
using AssessmentService.Application.Features.AssessmentQuestion.GetAllAssessmentQuestionByAssessmentId;
using AssessmentService.Application.Features.AssessmentQuestion.GetAssessmentQuestionById;
using AssessmentService.Application.Features.AssessmentQuestion.UpdateAssessmentQuestion;
using AssessmentService.Domain.Commons;
using Microsoft.AspNetCore.Mvc;

namespace AssessmentService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AssessmentQuestionController : Controller
    {
        private readonly ICommandHandler<CreateAssessmentQuestionCommand, int> _createAssessmentQuestionCommandHandler;
        private readonly ICommandHandler<UpdateAssessmentQuestionCommand, bool> _updateAssessmentQuestionCommandHandler;
        private readonly ICommandHandler<DeleteAssessmentQuestionCommand, bool> _deleteAssessmentQuestionCommandHandler;
        private readonly IQueryHandler<GetAssessmentQuestionByIdQuery, GetAssessmentQuestionByIdResponse> _getAssessmentQuestionByIdQueryHandler;
        private readonly IQueryHandler<GetAllAssessmentQuestionQuery, List<GetAllAssessmentQuestionResponse>> _getAllAssessmentQuestionQueryHandler;
        private readonly IQueryHandler<GetAllAssessmentQuestionByAssessmentIdQuery, List<GetAllAssessmentQuestionByAssessmentIdResponse>> _getAllAssessmentQuestionByAssessmentIdQueryHandler;

        public AssessmentQuestionController(ICommandHandler<CreateAssessmentQuestionCommand, int> createAssessmentQuestionCommandHandler,
            ICommandHandler<UpdateAssessmentQuestionCommand, bool> updateAssessmentQuestionCommandHandler,
            ICommandHandler<DeleteAssessmentQuestionCommand, bool> deleteAssessmentQuestionCommandHandler,
            IQueryHandler<GetAssessmentQuestionByIdQuery, GetAssessmentQuestionByIdResponse> getAssessmentQuestionByIdQueryHandler,
            IQueryHandler<GetAllAssessmentQuestionQuery, List<GetAllAssessmentQuestionResponse>> getAllAssessmentQuestionQueryHandler,
            IQueryHandler<GetAllAssessmentQuestionByAssessmentIdQuery, List<GetAllAssessmentQuestionByAssessmentIdResponse>> getAllAssessmentQuestionByAssessmentIdQueryHandler)
        {
            _createAssessmentQuestionCommandHandler = createAssessmentQuestionCommandHandler;
            _updateAssessmentQuestionCommandHandler = updateAssessmentQuestionCommandHandler;
            _deleteAssessmentQuestionCommandHandler = deleteAssessmentQuestionCommandHandler;
            _getAssessmentQuestionByIdQueryHandler = getAssessmentQuestionByIdQueryHandler;
            _getAllAssessmentQuestionQueryHandler = getAllAssessmentQuestionQueryHandler;
            _getAllAssessmentQuestionByAssessmentIdQueryHandler = getAllAssessmentQuestionByAssessmentIdQueryHandler;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ObjectResponse<GetAssessmentQuestionByIdResponse>>> GetAssessmentQuestionById(int id)
        {
            var result = await _getAssessmentQuestionByIdQueryHandler.Handle(new GetAssessmentQuestionByIdQuery(id), CancellationToken.None);
            if (result.Data is null)
            {
                if (result.ErrorCode == "404")
                    return NotFound(result);
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ObjectResponse<List<GetAllAssessmentQuestionResponse>>>> GetAllAssessmentQuestions()
        {
            var result = await _getAllAssessmentQuestionQueryHandler.Handle(new GetAllAssessmentQuestionQuery(), CancellationToken.None);
            return Ok(result);
        }

        [HttpGet("assessment/{assessmentId}")]
        public async Task<ActionResult<ObjectResponse<List<GetAllAssessmentQuestionByAssessmentIdResponse>>>> GetAllAssessmentQuestionsByAssessmentId(int assessmentId)
        {
            var result = await _getAllAssessmentQuestionByAssessmentIdQueryHandler.Handle(new GetAllAssessmentQuestionByAssessmentIdQuery(assessmentId), CancellationToken.None);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ObjectResponse<int>>> CreateAssessmentQuestion([FromBody] CreateAssessmentQuestionCommand command)
        {
            var result = await _createAssessmentQuestionCommandHandler.Handle(command, CancellationToken.None);
            return CreatedAtAction(nameof(GetAssessmentQuestionById), new { id = result.Data }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ObjectResponse<bool>>> UpdateAssessmentQuestion(int id, [FromBody] UpdateAssessmentQuestionCommand command)
        {
            if (id != command.AssessmentQuestionId)
            {
                return BadRequest(ObjectResponse<bool>.Response("400", "ID in URL does not match ID in body", false));
            }
            var result = await _updateAssessmentQuestionCommandHandler.Handle(command, CancellationToken.None);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ObjectResponse<bool>>> DeleteAssessmentQuestion(int id)
        {
            var result = await _deleteAssessmentQuestionCommandHandler.Handle(new DeleteAssessmentQuestionCommand(id), CancellationToken.None);
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
