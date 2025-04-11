using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;


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

        [Authorize(Policy = "TEACHER-REC-VIEW")]
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

        [Authorize(Policy = "TEACHER-REC-INSERT")]
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

        [Authorize(Policy = "TEACHER-REC-UPDATE")]
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

        [Authorize(Policy = "TEACHER-REC-DELETE")]
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

        [Authorize(Policy = "TEACHER-REC-VIEW")]
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

        [Authorize(Policy = "TEACHER-REC-VIEW")]
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

        [Authorize(Policy = "TEACHER-REC-VIEW")]
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

        [Authorize(Policy = "TEACHER-REC-VIEW")]
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

        [Authorize(Policy = "TEACHER-REC-VIEW")]
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

        [HttpGet("get-all-teachers")]
        public async Task<ActionResult<ApiResponse<List<UserResponseTeachingAssignment>>>> GetTeachers()
        {
            try
            {
                var teachers = await _teacherService.GetTeachersAsync();

                if (teachers == null || !teachers.Any())
                {
                    return Ok(new ApiResponse<List<UserResponseTeachingAssignment>>(1, "Không có giảng viên nào.", null));
                }

                return Ok(new ApiResponse<List<UserResponseTeachingAssignment>>(0, "Lấy danh sách giảng viên thành công!", teachers));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi lấy danh sách giảng viên.", ex.Message));
            }
        }

        [HttpGet("get-all-teacher-by-subjectId/{subjectId}")]
        public async Task<ActionResult<ApiResponse<List<UserResponseTeachingAssignment>>>>
GetTeacherBySubjectId(int subjectId)
        {
            try
            {
                var teacher = await _teacherService.GetTeacherBySubjectIdAsync(subjectId);

                if (teacher == null)
                {
                    return Ok(new ApiResponse<List<UserResponseTeachingAssignment>>(1, "Không có giảng viên nào.", null));
                }

                return Ok(new ApiResponse<List<UserResponseTeachingAssignment>>(0, "Lấy danh sách giảng viên thành công!", teacher));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi lấy danh sách giảng viên.", ex.Message));
            }
        }

    }
}
