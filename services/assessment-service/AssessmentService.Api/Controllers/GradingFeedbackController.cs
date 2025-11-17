using AssessmentService.Application.Abstractions.Messaging;
using AssessmentService.Application.Features.AssignmentAttempt.CreateAssignmentAttempt;
using AssessmentService.Application.Features.GradingFeedback.CalculateGrading;
using AssessmentService.Application.Features.GradingFeedback.GetGradingFeedback;
using AssessmentService.Domain.Commons;
using Microsoft.AspNetCore.Mvc;

namespace AssessmentService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GradingFeedbackController : Controller
    {
        private readonly ICommandHandler<CalculateGradingCommand, CalculateGradingResponse> _calculate;
        private readonly IQueryHandler<GetGradingFeedbackQuery, GetGradingFeedbackResponse> _get;

        public GradingFeedbackController(
            ICommandHandler<CalculateGradingCommand, CalculateGradingResponse> calculate,
            IQueryHandler<GetGradingFeedbackQuery, GetGradingFeedbackResponse> get)
        {
            _calculate = calculate;
            _get = get;
        }

        [HttpPost("calculate")]
        public async Task<ActionResult> CalculateGrading([FromBody] CalculateGradingCommand command)
        {
            var result = await _calculate.Handle(command, CancellationToken.None);
            if (result.Data != null)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ObjectResponse<GetGradingFeedbackResponse>>> GetGradingFeedback(int id)
        {
            var result = await _get.Handle(new GetGradingFeedbackQuery(id), CancellationToken.None);
            if (result.Data != null)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }
    }
}
