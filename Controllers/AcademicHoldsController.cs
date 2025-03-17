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
                "Success",
                result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var hold = await _academicHoldsService.GetByIdAcademicHold(id);
                if (hold == null) return NotFound();
                return Ok(new ApiResponse<AcademicHoldResponse>(0, "Success", hold));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<AcademicHoldResponse>(1, ex.Message, new AcademicHoldResponse()));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateAcademicHoldRequest academicHoldRequest)
        {
            try
            {
                await _academicHoldsService.AddAcademicHold(academicHoldRequest);
                return Ok(new ApiResponse<CreateAcademicHoldRequest>(0, "Add success", academicHoldRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<CreateAcademicHoldRequest>(1, ex.Message, new CreateAcademicHoldRequest()));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateAcademicHoldRequest academicHoldRequest)
        {
            try
            {
                await _academicHoldsService.UpdateAcademicHold(academicHoldRequest);
                return Ok(new ApiResponse<UpdateAcademicHoldRequest>(0, "Update success", academicHoldRequest));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<UpdateAcademicHoldRequest>(1, ex.Message, new UpdateAcademicHoldRequest()));
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            var result = await _academicHoldsService.DeleteAcademicHold(id);
            if (!result)
            {
                return NotFound(new ApiResponse<bool>(
                    1,
                    "Academic Year not found",
                    false));
            }

            return Ok(new ApiResponse<bool>(
                0,
                "Delete success",
                true));
        }
    }
}
