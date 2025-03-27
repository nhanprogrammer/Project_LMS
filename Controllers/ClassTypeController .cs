using Microsoft.AspNetCore.Authorization;
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
        private readonly IAuthService _authService;

        public ClassTypeController(IClassTypeService classTypeService, IAuthService authService)
        {
            _classTypeService = classTypeService;
            _authService = authService;
        }

        [Authorize(Policy = "DATA-MNG-VIEW")]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<ClassTypeResponse>>>> GetAll(
            [FromQuery] string? keyword = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var response = await _classTypeService.GetAllClassTypesAsync(keyword, pageNumber, pageSize);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [Authorize(Policy = "DATA-MNG-VIEW")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ClassTypeResponse>>> GetById(int id)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

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

        [Authorize(Policy = "DATA-MNG-INSERT")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ClassTypeResponse>>> Create([FromBody] ClassTypeRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var result = await _classTypeService.CreateClassTypeAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Internal server error: {ex.Message}", null));
            }
        }

        [Authorize(Policy = "DATA-MNG-UPDATE")]
        [HttpPut()]
        public async Task<ActionResult<ApiResponse<ClassTypeResponse>>> Update([FromBody] ClassTypeRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

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

        [Authorize(Policy = "DATA-MNG-DELETE")]
        [HttpDelete]
        public async Task<ActionResult<ApiResponse<bool>>> Delete([FromBody] DeleteMultipleRequest request)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

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
                return StatusCode(500, new ApiResponse<string>(1, $"Error deleting class types: {ex.Message}", null));
            }
        }
    }
}
