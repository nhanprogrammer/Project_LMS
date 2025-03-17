using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Services;

namespace Project_LMS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AcademicYearController : ControllerBase
    {
        private readonly IAcademicYearsService _academicYearsService;

        public AcademicYearController(IAcademicYearsService academicYearsService)
        {
            _academicYearsService = academicYearsService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<AcademicYearResponse>>> GetById(int id)
        {
            var result = await _academicYearsService.GetByIdAcademicYear(id);
            if (result == null)
            {
                return NotFound(new ApiResponse<AcademicYearResponse>(
                    1,
                    "Academic Year not found",
                    null));
            }

            return Ok(new ApiResponse<AcademicYearResponse>(
                0,
                "Success",
                result));
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<PaginatedResponse<AcademicYearResponse>>>> GetAll([FromQuery] PaginationRequest request)
        {
            var result = await _academicYearsService.GetPagedAcademicYears(request);
            return Ok(new ApiResponse<PaginatedResponse<AcademicYearResponse>>(
                0,
                "Success",
                result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CreateAcademicYearRequest>>> Add([FromBody] CreateAcademicYearRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new ApiResponse<CreateAcademicYearRequest>(
                        1,
                        "Request is null",
                        null));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<CreateAcademicYearRequest>(
                    1,
                    ex.Message,
                    null));
            }
            await _academicYearsService.AddAcademicYear(request);
            return Ok(new ApiResponse<CreateAcademicYearRequest>(
                0,
                "Add success",
                request));
        }

        [HttpPut]
        public async Task<ActionResult<ApiResponse<UpdateAcademicYearRequest>>> Update([FromBody] UpdateAcademicYearRequest request)
        {
            try
            {
                await _academicYearsService.UpdateAcademicYear(request);
                return Ok(new ApiResponse<UpdateAcademicYearRequest>(
                    0,
                    "Update success",
                    request));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<UpdateAcademicYearRequest>(
                    1,
                    ex.Message,
                    null));
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteLesson(DeleteRequest ids)
        {
            var response = await _academicYearsService.DeleteLessonAsync(ids);
            if (response.Status == 1)
            {
                return BadRequest(new ApiResponse<AcademicYearResponse>(response.Status, response.Message, response.Data));
            }

            return Ok(new ApiResponse<AcademicYearResponse>(response.Status, response.Message, response.Data));
        }
    }
}
