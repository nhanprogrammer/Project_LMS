using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers;

[Route("api/educationinformation")]
[ApiController]
public class EducationInformationController : ControllerBase
{
    private readonly IEducationInformationService _educationInformationService;

    public EducationInformationController(IEducationInformationService educationInformationService)
    {
        _educationInformationService = educationInformationService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll([FromQuery] EducationInformationRequest request)
    {
        try
        {
            var result = await _educationInformationService.GetAllAsync(request);
            return Ok(new ApiResponse<IEnumerable<EducationInformationsResponse>>(0, "Thành công", result));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(1, "Lỗi hệ thống: " + ex.Message, null));
        }
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById([FromQuery] EducationInformationDeleteRequest request)
    {
        try
        {
            var result = await _educationInformationService.GetById(request);
            if (result == null)
            {
                return NotFound(new ApiResponse<string>(1, "Không tìm thấy thông tin đào tạo.", null));
            }
            return Ok(new ApiResponse<EducationInformationResponse>(0, "Thành công", result));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(1, "Lỗi hệ thống: " + ex.Message, null));
        }
    }

    [HttpGet("trainingprograms/exclude")]
    public async Task<IActionResult> GetTrainingProgramsExcluding([FromQuery] TrainingProgramRequest request)
    {
        try
        {
            var result = await _educationInformationService.GetTrainingProgramsExcluding(request);
            return Ok(new ApiResponse<IEnumerable<TrainingProgramResponse>>(0, "Thành công", result));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(1, "Lỗi hệ thống: " + ex.Message, null));
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] EducationInformationCreateRequest request)
    {
        try
        {
            var result = await _educationInformationService.CreateAsync(request);
            return Ok(new ApiResponse<bool>(0, "Tạo thông tin đào tạo thành công", result));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<string>(1, ex.Message, null));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<string>(1, ex.Message, null));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(1, "Lỗi hệ thống: " + ex.Message, null));
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] EducationInformationUpdateRequest request)
    {
        try
        {
            var result = await _educationInformationService.UpdateAsync(request);
            return Ok(new ApiResponse<bool>(0, "Cập nhật thông tin đào tạo thành công", result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<string>(1, ex.Message, null));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<string>(1, ex.Message, null));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<string>(1, ex.Message, null));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(1, "Lỗi hệ thống: " + ex.Message, null));
        }
    }

    [HttpDelete("delete")]
    public async Task<IActionResult> Delete(EducationInformationDeleteRequest request)
    {
        try
        {
            var result = await _educationInformationService.DeleteAsync(request);
            return Ok(new ApiResponse<bool>(0, "Xóa thông tin đào tạo thành công", result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<string>(1, ex.Message, null));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<string>(1, ex.Message, null));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(1, "Lỗi hệ thống: " + ex.Message, null));
        }
    }
}
