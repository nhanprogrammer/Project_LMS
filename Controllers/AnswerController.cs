using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswersService _service;

        public AnswerController(IAnswersService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AnswerResponse>>> GetById(int id)
        {
            try
            {
                var result = await _service.GetAnswerById(id);
                if (result == null)
                {
                    return NotFound(new ApiResponse<AnswerResponse>(1, "Answer not found"));
                }
                return Ok(new ApiResponse<AnswerResponse>(0, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<AnswerResponse>>>> GetAll()
        {
            try
            {
                var result = await _service.GetAllAnswers();
                return Ok(new ApiResponse<IEnumerable<AnswerResponse>>(0, "Success", result));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<string>>> Add([FromBody] CreateAnswerRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Answer))
                {
                    return BadRequest(new ApiResponse<string>(1, "Vui lòng điền đầy đủ thông tin"));
                }

                await _service.AddAnswer(request);
                return CreatedAtAction(nameof(GetById), new { id = request.QuestionId }, new ApiResponse<string>(0, "Answer created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Update(int id, [FromBody] UpdateAnswerRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.Answer))
                {
                    return BadRequest(new ApiResponse<string>(1, "Invalid request data"));
                }

                var existingAnswer = await _service.GetAnswerById(id);
                if (existingAnswer == null)
                {
                    return NotFound(new ApiResponse<string>(1, "Answer not found"));
                }

                await _service.UpdateAnswer(id, request);
                return Ok(new ApiResponse<string>(0, "Answer updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> Delete(int id)
        {
            try
            {
                var deleted = await _service.DeleteAnswer(id);
                if (!deleted)
                {
                    return NotFound(new ApiResponse<string>(1, "Answer not found"));
                }
                return Ok(new ApiResponse<string>(0, "Answer deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}"));
            }
        }
    }
}
