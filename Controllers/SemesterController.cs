using Microsoft.AspNetCore.Mvc;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Helpers;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SemesterController : ControllerBase
    {
        private readonly ISemesterService _semesterService;

        public SemesterController(ISemesterService semesterService)
        {
            _semesterService = semesterService;
        }

        [HttpGet("by-academic-year/{academicYearId}")]
        public async Task<IActionResult> GetSemestersByAcademicYearId(int academicYearId)
        {
            try
            {
                var semesters = await _semesterService.GetSemestersByAcademicYearIdAsync(academicYearId);

                if (semesters == null || !semesters.Any())
                {
                    return Ok(new ApiResponse<List<SemesterDropdownResponse>>(1, "Không có học kỳ nào thuộc niên khóa này!", null));
                }

                return Ok(new ApiResponse<List<SemesterDropdownResponse>>(0, "Lấy danh sách học kỳ thành công!", semesters));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Lỗi hệ thống: {ex.Message}", null));
            }
        }
    }
}