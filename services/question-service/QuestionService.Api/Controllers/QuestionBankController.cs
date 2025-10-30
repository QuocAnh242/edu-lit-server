using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Api.Extensions;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;
using QuestionService.Application.DTOs.Request;
using QuestionService.Application.Features.QuestionBank.CreateQuestionBank;
using QuestionService.Application.Features.QuestionBank.DeleteQuestionBank;
using QuestionService.Application.Features.QuestionBank.GetAllQuestionBanks;
using QuestionService.Application.Features.QuestionBank.GetQuestionBankById;
using QuestionService.Application.Features.QuestionBank.GetQuestionBanksByOwnerId;
using QuestionService.Application.Features.QuestionBank.GetQuestionBanksBySubject;
using QuestionService.Application.Features.QuestionBank.UpdateQuestionBank;

namespace QuestionService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]  // Require authentication for all endpoints in this controller
    public class QuestionBankController : ControllerBase
    {
        private readonly ICommandHandler<CreateQuestionBankCommand, Guid> _createCommandHandler;
        private readonly ICommandHandler<UpdateQuestionBankCommand, Guid> _updateCommandHandler;
        private readonly ICommandHandler<DeleteQuestionBankCommand, bool> _deleteCommandHandler;
        private readonly IQueryHandler<GetQuestionBankByIdQuery, QuestionBankDto> _getByIdQueryHandler;
        private readonly IQueryHandler<GetAllQuestionBanksQuery, IEnumerable<QuestionBankDto>> _getAllQueryHandler;
        private readonly IQueryHandler<GetQuestionBanksByOwnerIdQuery, IEnumerable<QuestionBankDto>> _getByOwnerIdQueryHandler;
        private readonly IQueryHandler<GetQuestionBanksBySubjectQuery, IEnumerable<QuestionBankDto>> _getBySubjectQueryHandler;

        public QuestionBankController(
            ICommandHandler<CreateQuestionBankCommand, Guid> createCommandHandler,
            ICommandHandler<UpdateQuestionBankCommand, Guid> updateCommandHandler,
            ICommandHandler<DeleteQuestionBankCommand, bool> deleteCommandHandler,
            IQueryHandler<GetQuestionBankByIdQuery, QuestionBankDto> getByIdQueryHandler,
            IQueryHandler<GetAllQuestionBanksQuery, IEnumerable<QuestionBankDto>> getAllQueryHandler,
            IQueryHandler<GetQuestionBanksByOwnerIdQuery, IEnumerable<QuestionBankDto>> getByOwnerIdQueryHandler,
            IQueryHandler<GetQuestionBanksBySubjectQuery, IEnumerable<QuestionBankDto>> getBySubjectQueryHandler)
        {
            _createCommandHandler = createCommandHandler;
            _updateCommandHandler = updateCommandHandler;
            _deleteCommandHandler = deleteCommandHandler;
            _getByIdQueryHandler = getByIdQueryHandler;
            _getAllQueryHandler = getAllQueryHandler;
            _getByOwnerIdQueryHandler = getByOwnerIdQueryHandler;
            _getBySubjectQueryHandler = getBySubjectQueryHandler;
        }

        [HttpGet("{questionBanksId:guid}")]
        public async Task<IActionResult> GetById(Guid questionBanksId)
        {
            var query = new GetQuestionBankByIdQuery(questionBanksId);
            var res = await _getByIdQueryHandler.Handle(query, CancellationToken.None);
            if (!res.Success) return NotFound(res);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllQuestionBanksQuery();
            var res = await _getAllQueryHandler.Handle(query, CancellationToken.None);
            return Ok(res);
        }

        [HttpGet("owner/{ownerId:guid}")]
        public async Task<IActionResult> GetByOwnerId(Guid ownerId)
        {
            var query = new GetQuestionBanksByOwnerIdQuery(ownerId);
            var res = await _getByOwnerIdQueryHandler.Handle(query, CancellationToken.None);
            return Ok(res);
        }

        [HttpGet("subject/{subject}")]
        public async Task<IActionResult> GetBySubject(string subject)
        {
            var query = new GetQuestionBanksBySubjectQuery(subject);
            var res = await _getBySubjectQueryHandler.Handle(query, CancellationToken.None);
            return Ok(res);
        }

        /// <summary>
        /// Example: Get current user's question banks using JWT claims
        /// </summary>
        [HttpGet("my-question-banks")]
        public async Task<IActionResult> GetMyQuestionBanks()
        {
            // Extract user ID from JWT token
            var userId = User.GetUserId();
            
            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var query = new GetQuestionBanksByOwnerIdQuery(userId.Value);
            var res = await _getByOwnerIdQueryHandler.Handle(query, CancellationToken.None);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestionBankRequest request)
        {
            // Extract OwnerId from JWT token
            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var command = new CreateQuestionBankCommand
            {
                Title = request.Title,
                Description = request.Description,
                Subject = request.Subject,
                OwnerId = userId.Value
            };

            var res = await _createCommandHandler.Handle(command, CancellationToken.None);
            return Ok(res);
        }

        [HttpPut("{questionBanksId:guid}")]
        public async Task<IActionResult> Update(Guid questionBanksId, [FromBody] CreateQuestionBankRequest request)
        {
            // Extract OwnerId from JWT token
            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var command = new UpdateQuestionBankCommand
            {
                QuestionBankId = questionBanksId,
                Title = request.Title,
                Description = request.Description,
                Subject = request.Subject,
                OwnerId = userId.Value
            };
            var res = await _updateCommandHandler.Handle(command, CancellationToken.None);
            if (!res.Success) return NotFound(res);
            return Ok(res);
        }

        [HttpDelete("{questionBanksId:guid}")]
        public async Task<IActionResult> Delete(Guid questionBanksId)
        {
            var command = new DeleteQuestionBankCommand(questionBanksId);
            var res = await _deleteCommandHandler.Handle(command, CancellationToken.None);
            return Ok(res);
        }
    }
}
