using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Responsitories;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassStudentController : ControllerBase
    {
        private readonly IClassStudentService _classStudentService;
        private readonly IAuthService _authService;

        public ClassStudentController(IClassStudentService classStudentService, IAuthService authService)
        {
            _classStudentService = classStudentService;
            _authService = authService;
        }

        [HttpPost("changeclass")]
        public async Task<IActionResult> ChangeClass(ClassStudentRequest request)
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!"));
            var result = await _classStudentService.ChangeClassOfStudent(request, user.Id);
            return Ok(result);
        }
    }
}
