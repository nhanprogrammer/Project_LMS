using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
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
        public async Task<IActionResult> GetClassList([FromQuery] int AcademicYearId, [FromQuery] int DepartmentId, [FromQuery] int PageNumber = 1, [FromQuery] int PageSize = 10)
        {
            var response = await _classService.GetClassList(AcademicYearId, DepartmentId, PageNumber, PageSize);
            return Ok(response);
        }


        [HttpPost("save")]
        public async Task<IActionResult> SaveClass([FromBody] ClassSaveRequest request)
        {
            await _classService.SaveClass(request);
            return Ok(new ApiResponse<string>(0, "Lưu lớp học thành công", null));
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteClass([FromQuery] List<int> classId)
        {
            bool isDeleted = await _classService.DeleteClass(classId);
            if (isDeleted)
                return Ok(new ApiResponse<string>(0, "Xóa lớp học thành công", null));
            return NotFound(new ApiResponse<string>(1, "Không tìm thấy lớp học", null));
        }

        [HttpGet("detail/{classId}")]
        public async Task<IActionResult> GetClassDetail(int classId)
        {
            var response = await _classService.GetClassDetail(classId);
            return Ok(response);
        }

        [HttpGet("subjects/excluding")]
        public async Task<IActionResult> GetSubjectsExcluding([FromQuery] List<int> excludedSubjectIds)
        {
            var response = await _classService.GetSubjectsExcluding(excludedSubjectIds);
            return Ok(response);
        }

        [HttpGet("subjects/inherited")]
        public async Task<IActionResult> GetInheritedSubjects([FromQuery] int academicYearId, [FromQuery] int departmentId)
        {
            var response = await _classService.GetInheritedSubjects(academicYearId, departmentId);
            return Ok(response);
        }

        [HttpPut("student-status")]
        public async Task<IActionResult> SaveStudentStatus([FromQuery] int studentId, [FromQuery] int statusId)
        {
            bool isUpdated = await _classService.SaveStudentStatus(studentId, statusId);
            if (isUpdated)
                return Ok(new ApiResponse<string>(0, "Cập nhật trạng thái học sinh thành công", null));
            return BadRequest(new ApiResponse<string>(1, "Cập nhật trạng thái học sinh thất bại", null));
        }

        [HttpGet("export-class-list")]
        public async Task<IActionResult> ExportClassListToExcel([FromQuery] int academicYearId, [FromQuery] int departmentId)
        {
            var fileResult = await _classService.ExportClassListToExcel(academicYearId, departmentId);
            return fileResult;
        }
        
        [HttpPost("upload")]
        public async Task<IActionResult> UploadClassFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new ApiResponse<string>(1, "Vui lòng chọn file Excel hợp lệ.", null));
            }

            try
            {
                await _classService.CreateClassByFile(file);
                return Ok(new ApiResponse<string>(0, "Tải lên và xử lý file thành công!", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, "Xử lý file thất bại: " + ex.Message, null));
            }
        }

    }
}
