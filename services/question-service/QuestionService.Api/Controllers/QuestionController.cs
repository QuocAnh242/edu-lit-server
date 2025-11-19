using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Api.Extensions;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;
using QuestionService.Application.DTOs.Request;
using QuestionService.Application.Features.Question.CreateQuestion;
using QuestionService.Application.Features.Question.DeleteQuestion;
using QuestionService.Application.Features.Question.GetAllQuestions;
using QuestionService.Application.Features.Question.GetPublishedQuestions;
using QuestionService.Application.Features.Question.GetQuestionById;
using QuestionService.Application.Features.Question.GetQuestionsByAuthorId;
using QuestionService.Application.Features.Question.GetQuestionsByQuestionBankId;
using QuestionService.Application.Features.Question.GetQuestionsByType;
using QuestionService.Application.Features.Question.UpdateQuestion;
using QuestionService.Domain.Enums;

namespace QuestionService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/question")]
    [Authorize]  // Require authentication for all endpoints
    public class QuestionController : ControllerBase
    {
        private readonly ICommandHandler<CreateQuestionCommand, Guid> _createCommandHandler;
        private readonly ICommandHandler<UpdateQuestionCommand, Guid> _updateCommandHandler;
        private readonly ICommandHandler<DeleteQuestionCommand, bool> _deleteCommandHandler;
        private readonly IQueryHandler<GetQuestionByIdQuery, QuestionDto> _getByIdQueryHandler;
        private readonly IQueryHandler<GetAllQuestionsQuery, IEnumerable<QuestionDto>> _getAllQueryHandler;
        private readonly IQueryHandler<GetQuestionsByQuestionBankIdQuery, IEnumerable<QuestionDto>> _getByQuestionBankIdQueryHandler;
        private readonly IQueryHandler<GetQuestionsByAuthorIdQuery, IEnumerable<QuestionDto>> _getByAuthorIdQueryHandler;
        private readonly IQueryHandler<GetQuestionsByTypeQuery, IEnumerable<QuestionDto>> _getByTypeQueryHandler;
        private readonly IQueryHandler<GetPublishedQuestionsQuery, IEnumerable<QuestionDto>> _getPublishedQueryHandler;

        public QuestionController(
            ICommandHandler<CreateQuestionCommand, Guid> createCommandHandler,
            ICommandHandler<UpdateQuestionCommand, Guid> updateCommandHandler,
            ICommandHandler<DeleteQuestionCommand, bool> deleteCommandHandler,
            IQueryHandler<GetQuestionByIdQuery, QuestionDto> getByIdQueryHandler,
            IQueryHandler<GetAllQuestionsQuery, IEnumerable<QuestionDto>> getAllQueryHandler,
            IQueryHandler<GetQuestionsByQuestionBankIdQuery, IEnumerable<QuestionDto>> getByQuestionBankIdQueryHandler,
            IQueryHandler<GetQuestionsByAuthorIdQuery, IEnumerable<QuestionDto>> getByAuthorIdQueryHandler,
            IQueryHandler<GetQuestionsByTypeQuery, IEnumerable<QuestionDto>> getByTypeQueryHandler,
            IQueryHandler<GetPublishedQuestionsQuery, IEnumerable<QuestionDto>> getPublishedQueryHandler)
        {
            _createCommandHandler = createCommandHandler;
            _updateCommandHandler = updateCommandHandler;
            _deleteCommandHandler = deleteCommandHandler;
            _getByIdQueryHandler = getByIdQueryHandler;
            _getAllQueryHandler = getAllQueryHandler;
            _getByQuestionBankIdQueryHandler = getByQuestionBankIdQueryHandler;
            _getByAuthorIdQueryHandler = getByAuthorIdQueryHandler;
            _getByTypeQueryHandler = getByTypeQueryHandler;
            _getPublishedQueryHandler = getPublishedQueryHandler;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetQuestionByIdQuery(id);
            var res = await _getByIdQueryHandler.Handle(query, CancellationToken.None);
            if (!res.Success) return NotFound(res);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllQuestionsQuery();
            var res = await _getAllQueryHandler.Handle(query, CancellationToken.None);
            return Ok(res);
        }

        [HttpGet("question-bank/{id:guid}")]
        public async Task<IActionResult> GetByQuestionBankId(Guid id)
        {
            var query = new GetQuestionsByQuestionBankIdQuery(id);
            var res = await _getByQuestionBankIdQueryHandler.Handle(query, CancellationToken.None);
            return Ok(res);
        }

        [HttpGet("author/{id:guid}")]
        public async Task<IActionResult> GetByAuthorId(Guid id)
        {
            var query = new GetQuestionsByAuthorIdQuery(id);
            var res = await _getByAuthorIdQueryHandler.Handle(query, CancellationToken.None);
            return Ok(res);
        }

        [HttpGet("type/{id}")]
        public async Task<IActionResult> GetByQuestionType(QuestionType id)
        {
            var query = new GetQuestionsByTypeQuery(id);
            var res = await _getByTypeQueryHandler.Handle(query, CancellationToken.None);
            return Ok(res);
        }

        [HttpGet("published")]
        public async Task<IActionResult> GetPublishedQuestions()
        {
            var query = new GetPublishedQuestionsQuery();
            var res = await _getPublishedQueryHandler.Handle(query, CancellationToken.None);
            return Ok(res);
        }

        /// <summary>
        /// Get current user's questions using JWT claims
        /// </summary>
        [HttpGet("my-questions")]
        public async Task<IActionResult> GetMyQuestions()
        {
            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var query = new GetQuestionsByAuthorIdQuery(userId.Value);
            var res = await _getByAuthorIdQueryHandler.Handle(query, CancellationToken.None);
            return Ok(res);
        }

        [HttpPost]
        [Authorize(Roles = "TEACHER,ADMIN")]
        public async Task<IActionResult> Create([FromBody] CreateQuestionRequest request)
        {
            // Extract AuthorId from JWT token
            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var command = new CreateQuestionCommand
            {
                Title = request.Title,
                Body = request.Body,
                QuestionType = request.QuestionType,
                Metadata = request.Metadata,
                Tags = request.Tags,
                Version = request.Version,
                IsPublished = request.IsPublished,
                QuestionBankId = request.QuestionBankId,
                AuthorId = userId.Value
            };

            var res = await _createCommandHandler.Handle(command, CancellationToken.None);
            return Ok(res);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "TEACHER,ADMIN")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateQuestionRequest request)
        {
            // Extract AuthorId from JWT token
            var userId = User.GetUserId();
            if (userId == null)
            {
                return Unauthorized(new { message = "User ID not found in token" });
            }

            var command = new UpdateQuestionCommand
            {
                QuestionId = id,
                Title = request.Title,
                Body = request.Body,
                QuestionType = request.QuestionType,
                Metadata = request.Metadata,
                Tags = request.Tags,
                Version = request.Version,
                IsPublished = request.IsPublished,
                QuestionBankId = request.QuestionBankId,
                AuthorId = userId.Value
            };
            var res = await _updateCommandHandler.Handle(command, CancellationToken.None);
            if (!res.Success) return NotFound(res);
            return Ok(res);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "TEACHER,ADMIN")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteQuestionCommand(id);
            var res = await _deleteCommandHandler.Handle(command, CancellationToken.None);
            return Ok(res);
        }
    }
}
