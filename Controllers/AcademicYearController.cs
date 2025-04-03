using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Services;

namespace Project_LMS.Controllers
{
    [Authorize(Policy = "DATA-MNG-VIEW")]
    [ApiController]
    [Route("api/[controller]")]
    public class AcademicYearController : ControllerBase
    {
        private readonly IAcademicYearsService _academicYearsService;
        private readonly IAuthService _authService;

        public AcademicYearController(IAcademicYearsService academicYearsService, IAuthService authService)
        {
            _academicYearsService = academicYearsService;
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<AcademicYearResponse>>>> SearchAcademicYear(
            [FromQuery] int year)
        {
            var result = await _academicYearsService.SearchAcademicYear(year);

            if (result == null || !result.Items.Any())
            {
                return NotFound(
                    new ApiResponse<PaginatedResponse<AcademicYearResponse>>(1, "Không tìm thấy Niên khóa"));
            }

            return Ok(new ApiResponse<PaginatedResponse<AcademicYearResponse>>(0, "Lấy danh sách Niên khóa thành công",
                result));
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AcademicYearResponse>>> GetById(int id)
        {
            var result = await _academicYearsService.GetByIdAcademicYear(id);
            if (result.Status == 1)
            {
                return NotFound(new ApiResponse<AcademicYearWithSemestersDto>(
                    1,
                    $"Không tim thấy Niên Khóa có Id {id}"));
            }

            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<AcademicYearResponse>>>> GetAll(
     [FromQuery] PaginationRequest request,
     [FromQuery] string? keyword)
        {
            var result = await _academicYearsService.GetPagedAcademicYears(request, keyword);
            return Ok(new ApiResponse<PaginatedResponse<AcademicYearResponse>>(
                0,
                "Lấy danh sách Niên Khóa thành công",
                result));
        }
        [Authorize(Policy = "DATA-MNG-INSERT")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CreateAcademicYearRequest>>> Add(
            [FromBody] CreateAcademicYearRequest request)
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!"));

            var userId = user.Id;

            var result = await _academicYearsService.AddAcademicYear(request, userId);
            return Ok(result);
        }

        [Authorize(Policy = "DATA-MNG-UPDATE")]
        [HttpPut]
        public async Task<ActionResult<ApiResponse<UpdateAcademicYearRequest>>> Update(
            [FromBody] UpdateAcademicYearRequest request)
        {
            var user = await _authService.GetUserAsync();
            if (user == null)
                return Unauthorized(new ApiResponse<string>(1, "Token không hợp lệ hoặc đã hết hạn!", null));

            var userId = user.Id;
            var result = await _academicYearsService.UpdateAcademicYear(request, userId);
            return Ok(result);
        }

        [Authorize(Policy = "DATA-MNG-DELETE")]
        [HttpDelete]
        public async Task<IActionResult> DeleteLesson(DeleteRequest ids)
        {
            var response = await _academicYearsService.DeleteLessonAsync(ids);
            if (response.Status == 1)
            {
                return Ok(new ApiResponse<AcademicYearResponse>(response.Status, response.Message, response.Data));
            }

            return Ok(new ApiResponse<AcademicYearResponse>(response.Status, response.Message, response.Data));
        }

        [HttpGet("get-all-academic-years")]
        public async Task<ActionResult<ApiResponse<List<AcademicYearNameResponse>>>> GetAcademicYearNames()
        {
            var result = await _academicYearsService.GetAcademicYearNamesAsync();
            if (!result.Any())
            {
                return Ok(new ApiResponse<List<AcademicYearNameResponse>>(1, "Không tìm thấy niên khóa", null));
            }

            return Ok(new ApiResponse<List<AcademicYearNameResponse>>(0, "Lấy danh sách niên khóa thành công", result));
        }
    }
}