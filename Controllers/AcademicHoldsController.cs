using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Interfaces;
using Project_LMS.Models;
using Project_LMS.Services;

namespace Project_LMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicHoldsController : ControllerBase
    {
        private readonly IAcademicHoldsService _academicHoldsService;
        private readonly IAuthService _authService;
        public AcademicHoldsController(IAcademicHoldsService academicHoldsService, IAuthService authService)
        {
            _academicHoldsService = academicHoldsService;
            _authService = authService;
        }


        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<AcademicHoldResponse>>>> GetAll([FromQuery] PaginationRequest request)
        {

            try
            {
                var result = await _academicHoldsService.GetPagedAcademicHolds(request);
                return Ok(new ApiResponse<PaginatedResponse<AcademicHoldResponse>>(0, "Lấy danh sách bảo lưu thành công", result));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<PaginatedResponse<AcademicHoldResponse>>(1, "Lấy danh sách bảo lưu thất bại", null));
            }
        }

        [HttpGet("search-users")]
        public async Task<IActionResult> SearchUsers([FromQuery] int classId)
        {
            try
            {
                var users = await _academicHoldsService.SearchUsersByCriteriaAsync(classId);
                return Ok(new ApiResponse<List<User_AcademicHoldsResponse>>(
                    0,
                    "Tìm kiếm người dùng thành công!",
                    users));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, ex.Message, null));
            }
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateAcademicHoldRequest academicHoldRequest)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));
                var academicHold = await _academicHoldsService.AddAcademicHold(academicHoldRequest, user.Id);
                return Ok(new ApiResponse<AcademicHoldResponse>(0, "Thêm mới bảo lưu thành công", academicHold));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(1, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<CreateAcademicHoldRequest>(1, ex.Message, null));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAcademicHoldRequest academicHoldRequest)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));
                var academicHold = await _academicHoldsService.UpdateAcademicHold(academicHoldRequest, user.Id);
                return Ok(new ApiResponse<AcademicHoldResponse>(0, "Cập nhật bảo lưu thành công", academicHold));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(1, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<UpdateAcademicHoldRequest>(1, ex.Message, null));
            }
        }

        [HttpGet("semester-by-date")]
        public async Task<IActionResult> GetSemesterByDate([FromQuery] string date)
        {
            try
            {
                var user = await _authService.GetUserAsync();
                if (user == null)
                    return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

                var semester = await _academicHoldsService.GetSemesterByDateAsync(date);

                return Ok(new ApiResponse<SemesterResponse>(0, "Lấy thông tin học kỳ thành công!", semester));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, ex.Message, null));
            }
        }

    }
}
