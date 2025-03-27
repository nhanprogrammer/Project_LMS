using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Response;


namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;
        private readonly IStudentService _studentService;

        public TeacherController(ITeacherService teacherService, IStudentService studentService, IValidator<TeacherRequest> validator)
        {
            _teacherService = teacherService;
            _studentService = studentService;
        }

        [HttpGet("{usercode}")]
        public async Task<IActionResult> GetByUserCode(string? usercode)
        {
            try
            {
                var result = await _teacherService.FindTeacherByUserCode(usercode);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync(TeacherRequest request)
        {
            try
            {

                var result = await _teacherService.AddAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync(TeacherRequest request)
        {
            try
            {

                var result = await _teacherService.UpdateAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteTeacherRequest request)
        {
            try
            {
                var result = await _teacherService.DeleteAsync(request.UserCodes);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAll(int academicId, [FromQuery] PaginationRequest request, bool orderBy, string column)
        {
            try
            {
                var result = await _teacherService.GetAllByAcademic(academicId, request, orderBy, column, null);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet("getall/search")]
        public async Task<IActionResult> GetAllSearch(int academicId, [FromQuery] PaginationRequest request, bool orderBy, string column, string searchItem)
        {
            try
            {
                var result = await _teacherService.GetAllByAcademic(academicId, request, orderBy, column, searchItem);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet("getall/export")]
        public async Task<IActionResult> ExportExcel(int academicId, bool orderBy, string column)
        {
            try
            {
                var result = await _teacherService.ExportExcelByAcademic(academicId, orderBy, column, null);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet("getall/export/search")]
        public async Task<IActionResult> ExportExcelSearch(int academicId, bool orderBy, string column, string searchItem)
        {
            try
            {
                var result = await _teacherService.ExportExcelByAcademic(academicId, orderBy, column, searchItem);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }

        [HttpGet("generateusercode")]
        public async Task<IActionResult> GenerateUsercode()
        {
            try
            {
                var result = await _studentService.GeneratedUserCode("GV");
                return Ok(new ApiResponse<object>(0, "Lấy UserCode thành công")
                {
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi server: {ex.Message}");
            }
        }
    }
}
