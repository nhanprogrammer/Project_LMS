using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;

namespace Project_LMS.Controllers
{
    [Authorize(Policy = "DATA-MNG-VIEW")]
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentsService _departmentsService;

        public DepartmentController(IDepartmentsService departmentsService)
        {
            _departmentsService = departmentsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartment([FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            [FromQuery] string? sortDirection)
        {
            // Gọi service để lấy danh sách phòng ban
            var response = await _departmentsService.GetAllCoursesAsync(pageNumber, pageSize, sortDirection);

            // Nếu có lỗi (Status = 1), trả về BadRequest với thông báo và dữ liệu tương ứng
            if (response.Status == 1)
            {
                return BadRequest(new ApiResponse<PaginatedResponse<DepartmentResponse>>(
                    response.Status,
                    response.Message,
                    response.Data
                ));
            }

            // Nếu thành công, trả về Ok với thông báo và dữ liệu
            return Ok(new ApiResponse<PaginatedResponse<DepartmentResponse>>(
                response.Status,
                response.Message,
                response.Data
            ));
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchDepartments
        (
            [FromQuery] string? keyword,
            [FromQuery] int? pageNumber,
            [FromQuery] int? pageSize,
            [FromQuery] string? sortDirection
           )
        {
            var search = await _departmentsService.SearchDepartmentsAsync(keyword, pageNumber, pageSize, sortDirection);

            return Ok(search);
        }

        [HttpGet("departments/{id:int}/classes")]
        public async Task<IActionResult> GetClassesByDepartmentId([FromRoute] int id)
        {
            var listClass = await _departmentsService.GetAllClassesAsync(id);
            return Ok(listClass);
        }

        [Authorize(Policy = "DATA-MNG-INSERT")]
        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentRequest request)
        {
            var response = await _departmentsService.CreateDepartmentAsync(request);

            if (response.Status == 1)
            {
                return BadRequest(
                    new ApiResponse<DepartmentResponse>(response.Status, response.Message, response.Data));
            }

            return Ok(new ApiResponse<DepartmentResponse>(response.Status, response.Message, response.Data));
        }

        [Authorize(Policy = "DATA-MNG-UPDATE")]
        [HttpPut]
        public async Task<IActionResult> UpdateDepartment([FromBody] UpdateDepartmentRequest request)
        {
            var response = await _departmentsService.UpdateDepartmentAsync(request);

            if (response.Status == 1)
            {
                return BadRequest(
                    new ApiResponse<DepartmentResponse>(response.Status, response.Message, response.Data));
            }

            return Ok(new ApiResponse<DepartmentResponse>(response.Status, response.Message, response.Data));
        }

        [Authorize(Policy = "DATA-MNG-DELETE")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDepartment(string id)
        {
            var response = await _departmentsService.DeleteDepartmentAsync(id);

            if (response.Status == 1)
            {
                return BadRequest(
                    new ApiResponse<DepartmentResponse>(response.Status, response.Message, response.Data));
            }

            return Ok(new ApiResponse<DepartmentResponse>(response.Status, response.Message, response.Data));
        }

        [Authorize(Policy = "DATA-MNG-DELETE")]
        [HttpDelete("batch-delete")]
        public async Task<IActionResult> DeleteClasses([FromBody] DeleteRequest request)
        {
            var response = await _departmentsService.DeleteClassById(request.ids);
            return response.Status == 0 ? Ok(response) : BadRequest(response);
        }

    }
}