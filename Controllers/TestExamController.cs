using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;
using Project_LMS.Models;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestExamController : ControllerBase
    {
        private readonly ITestExamService _testExamService;

        public TestExamController(ITestExamService testExamService)
        {
            _testExamService = testExamService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDisciplinesAsync( string? keyword,   int? pageNumber,
            int? pageSize,
            string? sortDirection)
        {
            var response = await _testExamService.GetAllTestExamsAsync(keyword, pageNumber, pageSize, sortDirection);

            if (response.Status == 1)
            {
                return BadRequest(
                    new ApiResponse<PaginatedResponse<TestExamResponse>>(response.Status, response.Message, response.Data));
            }

            return Ok(new ApiResponse<PaginatedResponse<TestExamResponse>>(response.Status, response.Message, response.Data));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TestExamResponse>>> GetById(int id)
        {
            try
            {
                var response = await _testExamService.GetTestExamByIdAsync(id);
                if (response.Status == 1)
                {
                    return BadRequest(new ApiResponse<TestExamResponse>(response.Status, response.Message, response.Data));
                }

                return Ok(new ApiResponse<TestExamResponse>(response.Status, response.Message, response.Data));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TestExamResponse>>> Create([FromBody] TestExamRequest request)
        {
            try
            {
                var result = await _testExamService.CreateTestExamAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TestExamResponse>>> Update(int id, [FromBody] TestExamRequest request)
        {
            try
            {
                var result = await _testExamService.UpdateTestExamAsync(id, request);
                if (result.Data == null)
                {
                    return NotFound(new ApiResponse<TestExamResponse>(1, "TestExam not found", null));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            try
            {
                var result = await _testExamService.DeleteTestExamAsync(id);
                if (!result.Data)
                {
                    return NotFound(new ApiResponse<bool>(1, "TestExam not found", false));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }
    }

}
