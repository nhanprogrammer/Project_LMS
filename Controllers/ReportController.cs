using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("academic-year")]
        public async Task<ActionResult<ApiResponse<AcademicYearReportResponse>>> GetAcademicYearOverviewAsync([FromQuery] int academicId)
        {
            try
            {
                var report = await _reportService.GetAcademicYearOverviewAsync(academicId);
                return Ok(new ApiResponse<AcademicYearReportResponse>(0, "Lấy báo cáo thành công", report));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi lấy báo cáo", ex.Message));
            }
        }

        [HttpGet("class-performance")]
        public async Task<ActionResult<ApiResponse<List<ClassPerformanceReport>>>> GetClassPerformanceReport([FromQuery] int academicYearId, [FromQuery] int departmentId)
        {
            try
            {
                var report = await _reportService.GetClassPerformanceReportAsync(academicYearId, departmentId);
                return Ok(new ApiResponse<List<ClassPerformanceReport>>(0, "Lấy báo cáo thành công", report));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi lấy báo cáo", ex.Message));
            }
        }

        [HttpGet("school-level-statistics")]
        public async Task<ActionResult<ApiResponse<SchoolLevelStatisticsResponse>>> GetSchoolLevelStatistics(
    [FromQuery] int academicYearId, [FromQuery] bool isJuniorHigh)
        {
            try
            {
                var statistics = await _reportService.GetSchoolLevelStatisticsAsync(academicYearId, isJuniorHigh);
                return Ok(new ApiResponse<SchoolLevelStatisticsResponse>(0, "Lấy thống kê thành công", statistics));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi lấy thống kê", ex.Message));
            }
        }

        // Thống kê Teacher
        [HttpGet("teacher-statistics")]
        public async Task<ActionResult<ApiResponse<TeacherStatisticsResponse>>> GetTeacherStatistics([FromQuery] int userId)
        {
            try
            {
                var statistics = await _reportService.GetTeacherStatisticsAsync(userId);
                return Ok(new ApiResponse<TeacherStatisticsResponse>(0, "Lấy thống kê thành công", statistics));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi lấy thống kê", ex.Message));
            }
        }

        [HttpGet("teacher-performance")]
        public async Task<ActionResult<ApiResponse<TeacherPerformanceReport>>> GetTeacherPerformanceReport([FromQuery] int userId)
        {
            try
            {
                var report = await _reportService.GetTeacherPerformanceReportAsync(userId);
                return Ok(new ApiResponse<TeacherPerformanceReport>(0, "Lấy báo cáo thành công", report));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi lấy báo cáo", ex.Message));
            }
        }

        [HttpGet("teacher-semester-statistics")]
        public async Task<ActionResult<ApiResponse<List<TeacherSemesterStatisticsResponse>>>> GetTeacherSemesterStatistics([FromQuery] int teacherId)
        {
            try
            {
                var statistics = await _reportService.GetTeacherSemesterStatisticsAsync(teacherId);
                return Ok(new ApiResponse<List<TeacherSemesterStatisticsResponse>>(0, "Lấy thống kê thành công", statistics));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi lấy thống kê", ex.Message));
            }
        }
    }
}