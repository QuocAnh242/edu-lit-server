using Microsoft.AspNetCore.Mvc;
using QuestionService.Application.DTOs.Request;
using QuestionService.Application.DTOs.Response;
using QuestionService.Application.Services.Interfaces;

namespace QuestionService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class QuestionBankController : ControllerBase
    {
        private readonly IQuestionBankService _questionBankService;

        public QuestionBankController(IQuestionBankService questionBankService)
        {
            _questionBankService = questionBankService;
        }

        [HttpGet("{questionBanksId:guid}")]
        public async Task<IActionResult> GetById(Guid questionBanksId)
        {
            var res = await _questionBankService.GetByIdAsync(questionBanksId);
            if (!res.Success) return NotFound(res);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var res = await _questionBankService.GetAllAsync();
            return Ok(res);
        }

        [HttpGet("owner/{ownerId:guid}")]
        public async Task<IActionResult> GetByOwnerId(Guid ownerId)
        {
            var res = await _questionBankService.GetByOwnerIdAsync(ownerId);
            return Ok(res);
        }

        [HttpGet("subject/{subject}")]
        public async Task<IActionResult> GetBySubject(string subject)
        {
            var res = await _questionBankService.GetBySubjectAsync(subject);
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuestionBankRequest request)
        {
            var res = await _questionBankService.CreateAsync(request);
            return Ok(res);
        }

        [HttpPut("{questionBanksId:guid}")]
        public async Task<IActionResult> Update(Guid questionBanksId, [FromBody] CreateQuestionBankRequest request)
        {
            var res = await _questionBankService.UpdateAsync(questionBanksId, request);
            if (!res.Success) return NotFound(res);
            return Ok(res);
        }

        [HttpDelete("{questionBanksId:guid}")]
        public async Task<IActionResult> Delete(Guid questionBanksId)
        {
            var res = await _questionBankService.DeleteAsync(questionBanksId);
            return Ok(res);
        }
    }
}
