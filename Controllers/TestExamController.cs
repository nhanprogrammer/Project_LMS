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
        public async Task<ActionResult<ApiResponse<TestExamResponse>>> Create([FromBody] CreateTestExamRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var result = await _testExamService.CreateTestExamAsync(request);
            if (result.Status == 0)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TestExamResponse>>> Update
        (
            int id,
            [FromBody] UpdateTestExamRequest request
        )
        {
            try
            {
                var result = await _testExamService.UpdateTestExamAsync(id, request);
                if (result.Status == 1)
                {
                    return BadRequest(request);
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
                if (result.Status == 1)
                {
                    return NotFound(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<IEnumerable<object>>>> FilterClassAndYear
        (
            [FromQuery] int academicYearId,
            [FromQuery] int departmentId
        )
        {
            var response = await _testExamService.FilterClasses(academicYearId, departmentId);
            if (response.Status == 1)
                return BadRequest(response);
            return Ok(response);
        }

        [HttpGet("get-academic-years")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllAcademicYear()
        {
            var response = await _testExamService.GetAllAcademicYear();
            if (response.Status == 1)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("all-classes")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllClasses()
        {
            var response = await _testExamService.GetAllClasses();
            if (response.Status == 1)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("assignment-of-marking")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllAssignmentOfMarking()
        {
            var response = await _testExamService.GetAllAssignmentOfMarking();
            if (response.Status == 1)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}