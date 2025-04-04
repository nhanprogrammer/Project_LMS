using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Interfaces;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Project_LMS.Controllers
{
    [Route("api/class")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;
        private readonly IAuthService _authService;

        public ClassController(IClassService classService, IAuthService authService)
        {
            _classService = classService;
            _authService = authService;
        }

        [Authorize(Policy = "DATA-MNG-VIEW")]
        [HttpGet("list")]
        public async Task<IActionResult> GetClassList([FromQuery] ClassRequest classRequest)
        {
            var response = await _classService.GetClassList(classRequest);
            return Ok(response);
        }

        [Authorize(Policy = "DATA-MNG-INSERT")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateClass([FromBody] ClassSaveRequest request)
        {
            try
            {
                await _classService.SaveClass(request);
                return CreatedAtAction(nameof(CreateClass), new ApiResponse<string>(0, "Tạo lớp học thành công", null));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(1, new ApiResponse<string>(1, "Đã xảy ra lỗi, vui lòng thử lại.", null));
            }
        }

        [Authorize(Policy = "DATA-MNG-UPDATE")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateClass([FromBody] ClassSaveRequest request)
        {
            try
            {
                await _classService.SaveClass(request);
                return Ok(new ApiResponse<string>(0, "Cập nhật lớp học thành công", null));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(1, new ApiResponse<string>(1, "Đã xảy ra lỗi, vui lòng thử lại.", null));
            }
        }

        [Authorize(Policy = "DATA-MNG-DELETE")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteClass([FromBody] ClassListIdRequest request)
        {
            if (request?.Ids == null || !request.Ids.Any())
            {
                return BadRequest(new ApiResponse<string>(1, "Danh sách ID lớp học không hợp lệ.", null));
            }

            try
            {
                bool isDeleted = await _classService.DeleteClass(request.Ids);

                if (isDeleted)
                    return Ok(new ApiResponse<string>(0, "Xóa lớp học thành công", null));

                return NotFound(new ApiResponse<string>(1, "Không tìm thấy lớp học", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, ex.Message, null));
            }
        }

        [Authorize(Policy = "DATA-MNG-VIEW")]
        [HttpGet("detail")]
        public async Task<IActionResult> GetClassDetail([FromQuery] ClassIdRequest classId)
        {
            try
            {
                var response = await _classService.GetClassDetail(classId.Id);

                return Ok(new ApiResponse<object>(0, "Lấy thông tin lớp học thành công.", response));
            }
            catch (NotFoundException ex)
            {
                return StatusCode(404, new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi, vui lòng thử lại." + ex.Message, null));
            }
        }

        [Authorize(Policy = "DATA-MNG-VIEW")]
        [HttpGet("subjects/excluding")]
        public async Task<IActionResult> GetSubjectsExcluding([FromQuery] ClassListStringId request)
        {
            var response = await _classService.GetSubjectsExcluding(request.ids);
            return Ok(response);
        }

        [Authorize(Policy = "DATA-MNG-VIEW")]
        [HttpGet("subjects/inherited")]
        public async Task<IActionResult> GetInheritedSubjects([FromQuery] ClassAcademicDepartmentRequest request)
        {
            var response = await _classService.GetInheritedSubjects(request.AcademicYearId, request.DepartmentId);
            return Ok(response);
        }

        [Authorize(Policy = "DATA-MNG-UPDATE")]
        [HttpPut("student-status")]
        public async Task<IActionResult> SaveStudentStatus([FromBody] ClassStudentStatusRequest request)
        {
            if (request == null || request.StudentId <= 0 || request.StatusId <= 0)
            {
                return BadRequest(new ApiResponse<string>(1, "Dữ liệu không hợp lệ", null));
            }

            bool isUpdated = await _classService.SaveStudentStatus(request.StudentId, request.StatusId);
            if (isUpdated)
                return Ok(new ApiResponse<string>(0, "Cập nhật trạng thái học sinh thành công", null));

            return BadRequest(new ApiResponse<string>(1, "Cập nhật trạng thái học sinh thất bại", null));
        }

        [Authorize(Policy = "DATA-MNG-VIEW")]
        [HttpGet("export-class-list")]
        public async Task<IActionResult> ExportClassList(int academicYearId, int departmentId)
        {
            try
            {
                var base64String = await _classService.ExportClassListToExcel(academicYearId, departmentId);
                return Ok(new ApiResponse<string>(0, "Xuất danh sách lớp thành công!", base64String));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, "Lỗi khi xuất danh sách lớp: " + ex.Message, null));
            }
        }

        [Authorize(Policy = "DATA-MNG-INSERT")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadClassFile([FromBody] ClassBase64FileRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Base64File))
            {
                return BadRequest(new ApiResponse<string>(1, "Vui lòng cung cấp file Excel hợp lệ dưới dạng Base64.", null));
            }

            try
            {
                await _classService.CreateClassByBase64(request.Base64File);
                return Ok(new ApiResponse<string>(0, "Tải lên và xử lý file thành công!", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, "Xử lý file thất bại: " + ex.Message, null));
            }
        }

        [Authorize(Policy = "DATA-MNG-VIEW")]
        [HttpGet("download-excel")]
        public async Task<IActionResult> DownloadClassTemplate()
        {
            try
            {
                var base64String = await _classService.GenerateClassTemplate();
                return Ok(new ApiResponse<string>(0, "Tạo file mẫu thành công!", base64String));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, "Lỗi khi tạo file mẫu: " + ex.Message, null));
            }
        }

        [Authorize(Policy = "TEACHER")]
        [HttpGet("future")]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<ClassFutureResponse>>>> GetClassFuture(
    [FromQuery] string? keyword,
    [FromQuery] int? subjectId,
    [FromQuery] bool future,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var result = await _classService.GetClassFuture(user.Id, keyword, subjectId, future, pageNumber, pageSize);
                if (result.Status != 0)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Lỗi server: {ex.Message}", null));
            }
        }

        [Authorize(Policy = "TEACHER")]
        [HttpGet("future/{teachingAssignmentId}")]
        public async Task<ActionResult<ApiResponse<TeachingAssignmentDetailResponse>>> GetClassFutureDetail(int teachingAssignmentId)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var result = await _classService.GetClassFutureDetail(teachingAssignmentId);
                if (result.Status != 0)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Lỗi server: {ex.Message}", null));
            }
        }

        [Authorize(Policy = "STUDENT")]
        [HttpGet("futurestudent")]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<ClassFutureResponse>>>> GetClassLessonStudent(
      [FromQuery] string? keyword,
      [FromQuery] int? subjectId,
      [FromQuery] int status = 0,
      [FromQuery] int pageNumber = 1,
      [FromQuery] int pageSize = 10)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var result = await _classService.GetClassLessonStudent(user.Id, keyword, subjectId, status, pageNumber, pageSize);
                if (result.Status != 0)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Lỗi server: {ex.Message}", null));
            }
        }

        [Authorize(Policy = "STUDENT")]
        [HttpGet("futurestudent/{teachingAssignmentId}")]
        public async Task<ActionResult<ApiResponse<TeachingAssignmentDetailResponse>>> GetClassLessonStudentDetail(int teachingAssignmentId)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var result = await _classService.GetClassLessonStudentDetail(teachingAssignmentId);
                if (result.Status != 0)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Lỗi server: {ex.Message}", null));
            }
        }

        [HttpGet("search-classes")]
        public async Task<IActionResult> GetClassesByAcademicYear(
            [FromQuery] int academicYearId)
        {
            try
            {
                var classes = await _classService.GetClassesByAcademicYear(academicYearId);
                return Ok(new ApiResponse<List<Class_UserResponse>>(
                    0,
                    "Lấy danh sách lớp học thành công!",
                    classes));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, ex.Message, null));
            }
        }

        [HttpGet("get-all-classes")]
        public async Task<IActionResult> GetClassesDropdown([FromQuery] int academicYearId, [FromQuery] int departmentId)
        {
            try
            {
                if (academicYearId <= 0 || departmentId <= 0)
                {
                    return BadRequest(new ApiResponse<string>(1, "ID niên khóa hoặc khoa khối không hợp lệ.", null));
                }

                var classes = await _classService.GetClassesDropdown(academicYearId, departmentId);

                if (classes == null || !classes.Any())
                {
                    return Ok(new ApiResponse<List<ClassDropdownResponse>>(1, "Không tìm thấy lớp học nào.", null));
                }

                return Ok(new ApiResponse<List<ClassDropdownResponse>>(0, "Lấy danh sách lớp học thành công!", classes));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, $"Lỗi server: {ex.Message}", null));
            }
        }
    }
}
