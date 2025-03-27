
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Authorize(Policy = "STUDENT-REC-VIEW")]
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IClassStudentService _classStudentService;
        private readonly IUserService _userService;
        private readonly IStudentService _studentService;
        public StudentController(IClassStudentService classStudentService, IUserService userService, IStudentService studentService)
        {
            _classStudentService = classStudentService;
            _userService = userService;
            _studentService = studentService;
        }
        [HttpGet("findstudentbyusercode")]
        public async Task<IActionResult> FindStudenByUserCode([FromQuery] string userCode)
        {
            var result = await _studentService.FindStudentByUserCodeAsync(userCode);
            return Ok(result);
        }

        [HttpGet("exportexcel")]
        public async Task<IActionResult> ExportExcel([FromQuery] int academicId, [FromQuery] int departmentId, [FromQuery] string column, [FromQuery] bool orderBy)
        {
            var result = await _classStudentService.ExportAllStudentExcel(academicId, departmentId, column, orderBy, null);
            return Ok(result);
        }
        [HttpGet("exportexcel/seach")]
        public async Task<IActionResult> ExportExcelSearch([FromQuery] int academicId, [FromQuery] int departmentId, [FromQuery] string column, [FromQuery] bool orderBy, [FromQuery] string searchItem)
        {
            var result = await _classStudentService.ExportAllStudentExcel(academicId, departmentId, column, orderBy, searchItem);
            return Ok(result);
        }
        [HttpGet("getall")]
        public Task<ApiResponse<PaginatedResponse<object>>> GetAll(int academicId, int departmentId, [FromQuery] PaginationRequest request, string column, bool orderBy)
        {
            return _classStudentService.GetAllByAcademicAndDepartment(academicId, departmentId, request, column, orderBy, null);
        }
        [HttpGet("search")]
        public Task<ApiResponse<PaginatedResponse<object>>> Search(int academicId, int departmentId, [FromQuery] PaginationRequest request, string column, bool orderBy, string search)
        {
            return _classStudentService.GetAllByAcademicAndDepartment(academicId, departmentId, request, column, orderBy, search);
        }


        [HttpPost("checkuser/{name}")]
        public async Task<IActionResult> CheckUser(string name)
        {
            var result = await _userService.CheckUser(name);
            return Ok(result);
        }
        [HttpPost("checkOTP/{otp}")]
        public async Task<IActionResult> CheckOTP(string otp)
        {
            var result = await _userService.CheckOTP(otp);
            return Ok(result);
        }

        [HttpPost("changepassword")]
        [Authorize]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            var result = await _userService.ChangePassword(request);
            return Ok(result);
        }
        //Create Student
        [Authorize(Policy = "STUDENT-REC-INSERT")]
        [HttpPost("add")]
        public async Task<IActionResult> AddAsync([FromBody] StudentRequest request)
        {
            var result = await _studentService.AddAsync(request);
            return Ok(result);
        }

        [Authorize(Policy = "STUDENT-REC-UPDATE")]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateAsync([FromBody] StudentRequest request)
        {
            var result = await _studentService.UpdateAsync(request);
            return Ok(result);
        }

        [Authorize(Policy = "STUDENT-REC-DELETE")]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAsync([FromBody] DeleteStudentRequest request)
        {
            var result = await _studentService.DeleteAsync(request.UserCode);
            return Ok(result);
        }

        [HttpGet("learningoutcomes")]
        public async Task<IActionResult> LearningOutcomes([FromQuery] int studentId, [FromQuery] int classId)
        {
            var result = await _studentService.LearningOutcomesOfStudent(studentId, classId);
            return Ok(result);
        }

        [Authorize(Policy = "STUDENT-REC-INSERT")]
        [HttpPost("importexcel")]
        public async Task<IActionResult> AddStudentByImportExcel([FromForm] IFormFile fileExcel)
        {
            var result = await _studentService.ReadStudentsFromExcelAsync(fileExcel);
            return Ok(result);
        }


        [HttpGet("exportexcelstudent")]
        public async Task<IActionResult> ExportExcel([FromQuery] int studentId, [FromQuery] int classId)
        {
            var result = await _studentService.ExportExcelLearningProcess(studentId, classId);
            return Ok(result);
        }
        [HttpGet("generateusercode")]
        public async Task<IActionResult> GenerateUserCode([FromQuery] bool isStudent)
        {
            string code;
            if (isStudent)
            {
                code = "SV";
            }
            else
            {
                code = "GV";
            }
            var result = await _studentService.GeneratedUserCode(code);
            return Ok(new ApiResponse<object>(0,"Lấy UserCode thành công")
            {
                Data = result
            });  
        }
    }
}
