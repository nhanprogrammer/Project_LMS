using Microsoft.AspNetCore.Mvc;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;
using Project_LMS.Models;
using Microsoft.AspNetCore.Authorization;

namespace Project_LMS.Controllers
{
    [Authorize(Policy = "STUDENT-REC-VIEW")]
    [ApiController]
    [Route("api/[controller]")]
    public class RewardController : ControllerBase
    {
        private readonly IRewardService _rewardService;
        private readonly IStudentService _studentService;

        public RewardController(IStudentService studentService, IRewardService rewardService)
        {
            _rewardService = rewardService;
            _studentService = studentService;
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<RewardResponse>>> GetById(int id)
        {
            var result = _rewardService.GetByIdAsync(id);
            return Ok(result);
        }

        [Authorize(Policy = "STUDENT-REC-INSERT")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<RewardResponse>>> Create(RewardRequest request)
        {
            try
            {
                var ressult = await _rewardService.AddAsync(request);
                return Ok(ressult);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(1, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi tạo phần thưởng", ex.Message));
            }
        }

        [Authorize(Policy = "STUDENT-REC-UPDATE")]
        [HttpPut]
        public async Task<ActionResult<ApiResponse<RewardResponse>>> Update(UpdateRewardRequest request)
        {
            try
            {
                var ressult = await _rewardService.UpdateAsync(request);
                return Ok(ressult);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(new ApiResponse<List<ValidationError>>(1, "Validation failed.", ex.Errors));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi cập nhật phần thưởng", ex.Message));
            }
        }

        [Authorize(Policy = "STUDENT-REC-DELETE")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<RewardResponse>>> Delete(int id)
        {
            try
            {
                var result = await _rewardService.DeleteAsync(id);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi xóa phần thưởng", ex.Message));
            }
        }

        [HttpGet("getallstudentreward")]
        public async Task<IActionResult> GetAllStudentOfReward([FromQuery] int academicId, [FromQuery] int departmentId, [FromQuery] PaginationRequest request, [FromQuery] string column, [FromQuery] bool orderBy)
        {
            try
            {
                var resutlt = await _studentService.GetAllStudentOfRewardOrDisciplines(true, academicId, departmentId, request, column, orderBy, null);
                return Ok(resutlt);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi lấy danh sách phần thưởng", ex.Message));
            }
        }

        [HttpGet("searchstudentreward")]
        public async Task<IActionResult> SearchStudentOfReward([FromQuery] int academicId, [FromQuery] int departmentId, [FromQuery] PaginationRequest request, [FromQuery] string column, [FromQuery] bool orderBy, [FromQuery] string searchItem)
        {

            try
            {
                var resutlt = await _studentService.GetAllStudentOfRewardOrDisciplines(true, academicId, departmentId, request, column, orderBy, searchItem);
                return Ok(resutlt);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiResponse<string>(1, ex.Message, null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>(1, "Đã xảy ra lỗi khi tìm kiếm phần thưởng", ex.Message));
            }
        }
    }
}