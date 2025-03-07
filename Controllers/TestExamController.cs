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
        public async Task<ActionResult<ApiResponse<PaginatedResponse<TestExamResponse>>>> GetAll(
            [FromQuery] string? keyword = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _testExamService.GetAllTestExamsAsync(keyword, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TestExamResponse>>> GetById(int id)
        {
            try
            {
                var result = await _testExamService.GetTestExamByIdAsync(id);
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
