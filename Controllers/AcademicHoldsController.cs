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
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var hold = await _academicHoldsService.GetAllAcademicHold();
                if (hold == null) return NotFound();
                return Ok(new ApiResponse<IEnumerable<AcademicHoldResponse>>(0, "Success", hold));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<IEnumerable<AcademicHoldResponse>>(1, ex.Message, new List<AcademicHoldResponse>()));
            }
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

        [HttpPut]
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
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _academicHoldsService.DeleteAcademicHold(id);
                return Ok(new ApiResponse<string>(0, "Delete success", "Delete success"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(1, ex.Message, "Delete fail"));
            }
        }
    }
}
