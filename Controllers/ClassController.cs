using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_LMS.Controllers
{
    [Route("api/class")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly IClassService _classService;

        public ClassController(IClassService classService)
        {
            _classService = classService;
        }

        [HttpGet("list")]
        public async Task<IActionResult> GetClassList([FromQuery] ClassRequest classRequest)
        {
            var response = await _classService.GetClassList(classRequest);
            return Ok(response);
        }


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


        [HttpGet("subjects/excluding")]
        public async Task<IActionResult> GetSubjectsExcluding([FromQuery] ClassListStringId request)
        {
            var response = await _classService.GetSubjectsExcluding(request.ids);
            return Ok(response);
        }

        [HttpGet("subjects/inherited")]
        public async Task<IActionResult> GetInheritedSubjects([FromQuery] ClassAcademicDepartmentRequest request)
        {
            var response = await _classService.GetInheritedSubjects(request.AcademicYearId, request.DepartmentId);
            return Ok(response);
        }

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

        [HttpGet("export-class-list")]
        public async Task<IActionResult> ExportClassList(int academicYearId, int departmentId)
        {
            var fileData = await _classService.ExportClassListToExcel(academicYearId, departmentId);

            return File(fileData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"ClassList_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
        }


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


        [HttpGet("download-excel")]
        public async Task<IActionResult> DownloadClassTemplate()
        {
            var fileBytes = await _classService.GenerateClassTemplate();

            if (fileBytes is { Length: > 0 }) // Kiểm tra dữ liệu hợp lệ
            {
                return File(fileBytes,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"ClassTemplate_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }

            return BadRequest("Không thể tạo file mẫu.");
        }


    }
}
