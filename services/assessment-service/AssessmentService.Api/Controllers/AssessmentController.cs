using AssessmentService.Application.DTOs.Request;
using AssessmentService.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AssessmentService.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AssessmentController : ControllerBase
    {
        private readonly IAssessmentService _service;
        public AssessmentController(IAssessmentService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AssessmentDTO dto)
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
        public async Task<IActionResult> Update([FromBody] AssessmentDTO dto)
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
