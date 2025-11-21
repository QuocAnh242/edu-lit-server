using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestionService.Application.Abstractions.Messaging;
using QuestionService.Application.DTOs;
using QuestionService.Application.Features.QuestionOption.CreateQuestionOption;
using QuestionService.Application.Features.QuestionOption.DeleteQuestionOption;
using QuestionService.Application.Features.QuestionOption.DeleteQuestionOptionsByQuestionId;
using QuestionService.Application.Features.QuestionOption.GetQuestionOptionById;
using QuestionService.Application.Features.QuestionOption.GetQuestionOptionsByQuestionId;
using QuestionService.Application.Features.QuestionOption.UpdateQuestionOption;

namespace QuestionService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/questionoption")]
    [Authorize]  // Require authentication for all endpoints
    public class QuestionOptionController : ControllerBase
    {
        private readonly ICommandHandler<CreateQuestionOptionCommand, Guid> _createCommandHandler;
        private readonly ICommandHandler<UpdateQuestionOptionCommand, Guid> _updateCommandHandler;
        private readonly ICommandHandler<DeleteQuestionOptionCommand, bool> _deleteCommandHandler;
        private readonly ICommandHandler<DeleteQuestionOptionsByQuestionIdCommand, bool> _deleteByQuestionIdCommandHandler;
        private readonly IQueryHandler<GetQuestionOptionByIdQuery, QuestionOptionDto> _getByIdQueryHandler;
        private readonly IQueryHandler<GetQuestionOptionsByQuestionIdQuery, IEnumerable<QuestionOptionDto>> _getByQuestionIdQueryHandler;

        public QuestionOptionController(
            ICommandHandler<CreateQuestionOptionCommand, Guid> createCommandHandler,
            ICommandHandler<UpdateQuestionOptionCommand, Guid> updateCommandHandler,
            ICommandHandler<DeleteQuestionOptionCommand, bool> deleteCommandHandler,
            ICommandHandler<DeleteQuestionOptionsByQuestionIdCommand, bool> deleteByQuestionIdCommandHandler,
            IQueryHandler<GetQuestionOptionByIdQuery, QuestionOptionDto> getByIdQueryHandler,
            IQueryHandler<GetQuestionOptionsByQuestionIdQuery, IEnumerable<QuestionOptionDto>> getByQuestionIdQueryHandler)
        {
            _createCommandHandler = createCommandHandler;
            _updateCommandHandler = updateCommandHandler;
            _deleteCommandHandler = deleteCommandHandler;
            _deleteByQuestionIdCommandHandler = deleteByQuestionIdCommandHandler;
            _getByIdQueryHandler = getByIdQueryHandler;
            _getByQuestionIdQueryHandler = getByQuestionIdQueryHandler;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var query = new GetQuestionOptionByIdQuery(id);
            var res = await _getByIdQueryHandler.Handle(query, CancellationToken.None);
            if (!res.Success) return NotFound(res);
            return Ok(res);
        }

        [HttpGet("question/{id:guid}")]
        public async Task<IActionResult> GetByQuestionId(Guid id)
        {
            var query = new GetQuestionOptionsByQuestionIdQuery(id);
            var res = await _getByQuestionIdQueryHandler.Handle(query, CancellationToken.None);
            return Ok(res);
        }

        [HttpPost]
        [Authorize(Roles = "TEACHER,ADMIN")]
        public async Task<IActionResult> Create([FromBody] CreateQuestionOptionCommand command)
        {
            var res = await _createCommandHandler.Handle(command, CancellationToken.None);
            return Ok(res);
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "TEACHER,ADMIN")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateQuestionOptionCommand request)
        {
            var command = new UpdateQuestionOptionCommand
            {
                QuestionOptionId = id,
                OptionText = request.OptionText,
                IsCorrect = request.IsCorrect,
                OrderIdx = request.OrderIdx
            };
            var res = await _updateCommandHandler.Handle(command, CancellationToken.None);
            if (!res.Success) return NotFound(res);
            return Ok(res);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "TEACHER,ADMIN")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteQuestionOptionCommand(id);
            var res = await _deleteCommandHandler.Handle(command, CancellationToken.None);
            return Ok(res);
        }

        [HttpDelete("question/{id:guid}")]
        [Authorize(Roles = "TEACHER,ADMIN")]
        public async Task<IActionResult> DeleteByQuestionId(Guid id)
        {
            var command = new DeleteQuestionOptionsByQuestionIdCommand(id);
            var res = await _deleteByQuestionIdCommandHandler.Handle(command, CancellationToken.None);
            return Ok(res);
        }
    }
}
