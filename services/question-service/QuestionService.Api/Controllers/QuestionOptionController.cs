using Microsoft.AspNetCore.Mvc;
using QuestionService.Application.DTOs.Request;
using QuestionService.Application.DTOs.Response;
using QuestionService.Application.Services.Interfaces;

namespace QuestionService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class QuestionOptionController : ControllerBase
    {
        private readonly IQuestionOptionService _questionOptionService;

        public QuestionOptionController(IQuestionOptionService questionOptionService)
        {
            _questionOptionService = questionOptionService;
        }

        [HttpGet("{questionOptionId:guid}")]
        public async Task<IActionResult> GetById(Guid questionOptionId)
        {
            var res = await _questionOptionService.GetByIdAsync(questionOptionId);
            if (!res.Success) return NotFound(res);
            return Ok(res);
        }

        [HttpGet("question/{questionId:guid}")]
        public async Task<IActionResult> GetByQuestionId(Guid questionId)
        {
            var res = await _questionOptionService.GetByQuestionIdAsync(questionId);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestionOptionRequest request)
        {
            var res = await _questionOptionService.CreateAsync(request);
            return Ok(res);
        }

        [HttpPut("{questionOptionId:guid}")]
        public async Task<IActionResult> Update(Guid questionOptionId, [FromBody] CreateQuestionOptionRequest request)
        {
            var res = await _questionOptionService.UpdateAsync(questionOptionId, request);
            if (!res.Success) return NotFound(res);
            return Ok(res);
        }

        [HttpDelete("{questionOptionId:guid}")]
        public async Task<IActionResult> Delete(Guid questionOptionId)
        {
            var res = await _questionOptionService.DeleteAsync(questionOptionId);
            return Ok(res);
        }

        [HttpDelete("question/{questionId:guid}")]
        public async Task<IActionResult> DeleteByQuestionId(Guid questionId)
        {
            var res = await _questionOptionService.DeleteByQuestionIdAsync(questionId);
            return Ok(res);
        }
    }
}
