using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClassTypeController : ControllerBase
    {
        private readonly IClassTypeService _classTypeService;

        public ClassTypeController(IClassTypeService classTypeService)
        {
            _classTypeService = classTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<ClassTypeResponse>>>> GetAll(
            [FromQuery] string? keyword = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _classTypeService.GetAllClassTypesAsync(keyword, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ClassTypeResponse>>> GetById(int id)
        {
            try
            {
                var result = await _classTypeService.GetClassTypeByIdAsync(id);
                if (result.Data == null)
                {
                    return NotFound(new ApiResponse<ClassTypeResponse>(1, "ClassType not found", null));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ClassTypeResponse>>> Create([FromBody] ClassTypeRequest request)
        {
            try
            {
                var result = await _classTypeService.CreateClassTypeAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<ClassTypeResponse>>> Update([FromBody] ClassTypeRequest request)
        {
            try
            {
                var result = await _classTypeService.UpdateClassTypeAsync(request);
                if (result.Data == null)
                {
                    return NotFound(new ApiResponse<ClassTypeResponse>(1, "ClassType not found", null));
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [HttpDelete]
        public async Task<ActionResult<ApiResponse<bool>>> Delete([FromBody] DeleteMultipleRequest request)
        {
            try
            {
                if (request?.Ids == null || !request.Ids.Any())
                {
                    return BadRequest(new ApiResponse<bool>(1, "No IDs provided", false));
                }

                var result = await _classTypeService.DeleteClassTypeAsync(request.Ids);
                if (result.Status == 0)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<bool>(1, $"Error deleting classtype: {ex.Message}", false));
            }
        }
    }
}
