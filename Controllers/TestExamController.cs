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
        public async Task<IActionResult> GetAllDisciplinesAsync(string? keyword, int? pageNumber, int? pageSize,
            string? sortDirection)
        {
            try
            {
                var response =
                    await _testExamService.GetAllTestExamsAsync(keyword, pageNumber, pageSize, sortDirection);

                if (response.Status == 1)
                {
                    return BadRequest(new ApiResponse<PaginatedResponse<TestExamResponse>>(response.Status,
                        response.Message, response.Data));
                }

                return Ok(new ApiResponse<PaginatedResponse<TestExamResponse>>(response.Status, response.Message,
                    response.Data));
            }
            catch (Exception ex)
            {
                // Log the exception (use a logging framework)
                return StatusCode(500, new ApiResponse<string>(1, "An unexpected error occurred.", ex.Message));
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TestExamResponse>>> GetById(int id)
        {
            try
            {
                var response = await _testExamService.GetTestExamByIdAsync(id);
                if (response.Status == 1)
                {
                    return BadRequest(
                        new ApiResponse<TestExamResponse>(response.Status, response.Message, response.Data));
                }

                return Ok(new ApiResponse<TestExamResponse>(response.Status, response.Message, response.Data));
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

        [HttpPut]
        public async Task<ActionResult<ApiResponse<TestExamResponse>>> Update
        (
            [FromBody] UpdateTestExamRequest request
        )
        {
            try
            {
                var result = await _testExamService.UpdateTestExamAsync(request);
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

        [HttpDelete("{id:int}")]
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