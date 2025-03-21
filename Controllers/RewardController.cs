using Microsoft.AspNetCore.Mvc;
using Project_LMS.Interfaces.Services;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Exceptions;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RewardController : ControllerBase
    {
        //private readonly IRewardService _rewardService;
        private readonly IStudentService _studentService;

        public RewardController( IStudentService studentService)
        {
            //_rewardService = rewardService;
            _studentService = studentService;
        }

        //[HttpGet]
        //public async Task<ActionResult<ApiResponse<IEnumerable<RewardResponse>>>> GetAll()
        //{
        //    try
        //    {
        //        var rewards = await _rewardService.GetAllAsync();
        //        return Ok(new ApiResponse<IEnumerable<RewardResponse>>(1, "Lấy danh sách phần thưởng thành công", rewards));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy danh sách phần thưởng", ex.Message));
        //    }
        //}

        //[HttpGet("{id}")]
        //public async Task<ActionResult<ApiResponse<RewardResponse>>> GetById(int id)
        //{
        //    try
        //    {
        //        var reward = await _rewardService.GetByIdAsync(id);
        //        if (reward == null)
        //        {
        //            return NotFound(new ApiResponse<RewardResponse>(0, "Không tìm thấy phần thưởng"));
        //        }
        //        return Ok(new ApiResponse<RewardResponse>(1, "Lấy phần thưởng thành công", reward));
        //    }
        //    catch (NotFoundException ex)
        //    {
        //        return NotFound(new ApiResponse<string>(0, ex.Message, null));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi lấy phần thưởng", ex.Message));
        //    }
        //}

        //[HttpPost]
        //public async Task<ActionResult<ApiResponse<RewardResponse>>> Create(RewardRequest request)
        //{
        //    try
        //    {
        //        var reward = await _rewardService.CreateAsync(request);
        //        return CreatedAtAction(nameof(GetById), new { id = reward.Id }, new ApiResponse<RewardResponse>(1, "Tạo phần thưởng thành công", reward));
        //    }
        //    catch (BadRequestException ex)
        //    {
        //        return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi tạo phần thưởng", ex.Message));
        //    }
        //}

        //[HttpPut("{id}")]
        //public async Task<ActionResult<ApiResponse<RewardResponse>>> Update(int id, RewardRequest request)
        //{
        //    try
        //    {
        //        var reward = await _rewardService.UpdateAsync(id, request);
        //        if (reward == null)
        //        {
        //            return NotFound(new ApiResponse<RewardResponse>(0, "Không tìm thấy phần thưởng"));
        //        }
        //        return Ok(new ApiResponse<RewardResponse>(1, "Cập nhật phần thưởng thành công", reward));
        //    }
        //    catch (NotFoundException ex)
        //    {
        //        return NotFound(new ApiResponse<string>(0, ex.Message, null));
        //    }
        //    catch (BadRequestException ex)
        //    {
        //        return BadRequest(new ApiResponse<List<ValidationError>>(400, "Validation failed.", ex.Errors));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi cập nhật phần thưởng", ex.Message));
        //    }
        //}

        //[HttpDelete("{id}")]
        //public async Task<ActionResult<ApiResponse<RewardResponse>>> Delete(int id)
        //{
        //    try
        //    {
        //        var reward = await _rewardService.DeleteAsync(id);
        //        if (reward == null)
        //        {
        //            return NotFound(new ApiResponse<RewardResponse>(0, "Không tìm thấy phần thưởng"));
        //        }
        //        return Ok(new ApiResponse<RewardResponse>(1, "Xóa phần thưởng thành công", reward));
        //    }
        //    catch (NotFoundException ex)
        //    {
        //        return NotFound(new ApiResponse<string>(0, ex.Message, null));
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new ApiResponse<string>(0, "Đã xảy ra lỗi khi xóa phần thưởng", ex.Message));
        //    }
        //}
        [HttpGet("getallstudentreward")]
        public async Task<IActionResult> GetAllStudentOfReward([FromQuery] int academicId, [FromQuery] int departmentId, [FromQuery] PaginationRequest request, [FromQuery] string column, [FromQuery] bool orderBy)
        {
            var resutlt = await _studentService.GetAllStudentOfRewardOrDisciplines(true,academicId, departmentId,request,column,orderBy,null);
            return Ok(resutlt);
        }   
        [HttpGet("searchstudentreward")]
        public async Task<IActionResult> SearchStudentOfReward([FromQuery] int academicId, [FromQuery] int departmentId, [FromQuery] PaginationRequest request, [FromQuery] string column, [FromQuery] bool orderBy,[FromQuery] string searchItem)
        {
            var resutlt = await _studentService.GetAllStudentOfRewardOrDisciplines(true,academicId,departmentId,request,column,orderBy, searchItem);
            return Ok(resutlt);
        }
    }
}