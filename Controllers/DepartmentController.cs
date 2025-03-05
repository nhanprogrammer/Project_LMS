using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;

namespace Project_LMS.Controllers
{
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
        public async Task<IActionResult> SearchDepartments([FromQuery] string? keyword)
        {
            var search = await _departmentsService.SearchDepartmentsAsync(keyword);

            return Ok(new ApiResponse<ApiResponse<IEnumerable<DepartmentResponse>>>(0, "Success", search));
        }

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

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentRequest request)
        {
            var response = await _departmentsService.UpdateDepartmentAsync(id, request);

            if (response.Status == 1)
            {
                return BadRequest(
                    new ApiResponse<DepartmentResponse>(response.Status, response.Message, response.Data));
            }

            return Ok(new ApiResponse<DepartmentResponse>(response.Status, response.Message, response.Data));
        }

        [HttpDelete("{id?}")]
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
    }
}