using Microsoft.AspNetCore.Mvc;
using QuestionService.Application.DTOs.Request;
using QuestionService.Application.DTOs.Response;
using QuestionService.Application.Services.Interfaces;

namespace QuestionService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var res = await _questionService.GetByIdAsync(id);
            if (!res.Success) return NotFound(res);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await _questionService.GetAllAsync();
            return Ok(res);
        }

        [HttpGet("question-bank/{questionBankId:guid}")]
        public async Task<IActionResult> GetByQuestionBankId(Guid questionBankId)
        {
            var res = await _questionService.GetByQuestionBankIdAsync(questionBankId);
            return Ok(res);
        }

        [HttpGet("author/{authorId:guid}")]
        public async Task<IActionResult> GetByAuthorId(Guid authorId)
        {
            var res = await _questionService.GetByAuthorIdAsync(authorId);
            return Ok(res);
        }

        [HttpGet("type/{questionType}")]
        public async Task<IActionResult> GetByQuestionType(string questionType)
        {
            var res = await _questionService.GetByQuestionTypeAsync(questionType);
            return Ok(res);
        }

        [HttpGet("published")]
        public async Task<IActionResult> GetPublishedQuestions()
        {
            var res = await _questionService.GetPublishedQuestionsAsync();
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestionRequest request)
        {
            var res = await _questionService.CreateAsync(request);
            return Ok(res);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateQuestionRequest request)
        {
            var res = await _questionService.UpdateAsync(id, request);
            if (!res.Success) return NotFound(res);
            return Ok(res);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await _questionService.DeleteAsync(id);
            return Ok(res);
        }
    }
}
