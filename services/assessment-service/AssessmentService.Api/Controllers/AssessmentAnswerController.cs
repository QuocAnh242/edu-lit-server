using AssessmentService.Database.Models.DTOs;
using AssessmentService.Service;
using Microsoft.AspNetCore.Mvc;

namespace AssessmentService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AssessmentAnswerController : ControllerBase
    {
        private readonly IAssessmentAnswerService _service;
        public AssessmentAnswerController(IAssessmentAnswerService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AssessmentAnswerDTO dto)
        {
            var response = await _service.CreateAsync(dto);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _service.GetAllAsync();
            return Ok(response);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var response = await _service.GetByIdAsync(id);
            return Ok(response);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] AssessmentAnswerDTO dto)
        {
            var response = await _service.UpdateAsync(dto);
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _service.DeleteAsync(id);
            return Ok(response);
        }
    }
}
