using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
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

        public AcademicHoldsController(IAcademicHoldsService academicHoldsService)
        {
            _academicHoldsService = academicHoldsService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<AcademicHoldResponse>>>> GetAll([FromQuery] PaginationRequest request)
        {
            var result = await _academicHoldsService.GetPagedAcademicHolds(request);
            return Ok(new ApiResponse<PaginatedResponse<AcademicHoldResponse>>(
                0,
                "Thành công",
                result));
        }

        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetById(int id)
        //{
        //    try
        //    {
        //        var hold = await _academicHoldsService.GetById(id);
        //        if (hold == null) return NotFound();
        //        return Ok(new ApiResponse<User_AcademicHoldResponse>(0, "Thành công", hold));
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new ApiResponse<AcademicHoldResponse>(1, ex.Message, new AcademicHoldResponse()));
        //    }
        //}

        [HttpGet("classesById")]
        public async Task<IActionResult> GetAllClasses()
        {
            var classes = await _academicHoldsService.GetAllUser_Class();
            return Ok(new ApiResponse<List<Class_UserResponse>>(
                0,
                "Thành công",
                classes));
        }


        [HttpGet("UserById")]
        public async Task<IActionResult> GetAllUser()
        {
            var classes = await _academicHoldsService.GetAllUserName();
            return Ok(new ApiResponse<List<User_AcademicHoldsResponse>>(
                0,
                "Thành công",
                classes));
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateAcademicHoldRequest academicHoldRequest)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("UserId")?.Value ?? "1");
                academicHoldRequest.UserCreate = userId;
                await _academicHoldsService.AddAcademicHold(academicHoldRequest);
                return Ok(new ApiResponse<CreateAcademicHoldRequest>(0, "Thêm mới thành công", academicHoldRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<CreateAcademicHoldRequest>(1, ex.Message, new CreateAcademicHoldRequest()));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateAcademicHoldRequest academicHoldRequest)
        {
            try
            {
                await _academicHoldsService.UpdateAcademicHold(academicHoldRequest);
                return Ok(new ApiResponse<UpdateAcademicHoldRequest>(0, "Cập nhật thành công", academicHoldRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<UpdateAcademicHoldRequest>(1, ex.Message, new UpdateAcademicHoldRequest()));
            }
        }

        //[HttpDelete("{id}")]
        //public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        //{
        //    var result = await _academicHoldsService.DeleteAcademicHold(id);
        //    if (!result)
        //    {
        //        return NotFound(new ApiResponse<bool>(
        //            1,
        //            "Academic Year not found",
        //            false));
        //    }

        //    return Ok(new ApiResponse<bool>(
        //        0,
        //        "Delete success",
        //        true));
        //}
    }
}
