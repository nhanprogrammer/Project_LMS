using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers;

[Route("api/workprocess")]
[ApiController]
public class WorkProcessController : ControllerBase
{
    private readonly IWorkProcessService _workProcessService;

    public WorkProcessController(IWorkProcessService workProcessService)
    {
        _workProcessService = workProcessService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll([FromQuery] WorkProcessRequest request)
    {
        try
        {
            var result = await _workProcessService.GetAllAsync(request);
            return Ok(new ApiResponse<IEnumerable<WorkProcessesResponse>>(0, "Thành công", result));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(1, "Lỗi hệ thống: " + ex.Message, null));
        }
    }

    [HttpGet("get-by-id")]
    public async Task<IActionResult> GetById([FromQuery] WorkProcessDeleteRequest request)
    {
        try
        {
            var result = await _workProcessService.GetById(request);
            if (result == null)
            {
                return NotFound(new ApiResponse<string>(1, "Không tìm thấy quá trình làm việc.", null));
            }
            return Ok(new ApiResponse<WorkProcessResponse>(0, "Thành công", result));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(1, "Lỗi hệ thống: " + ex.Message, null));
        }
    }

    [HttpGet("workunits/exclude")]
    public async Task<IActionResult> GetWorkUnitExcluding([FromQuery] WorkUnitRequest request)
    {
        try
        {
            var result = await _workProcessService.GetWorkUnitExcluding(request);
            return Ok(new ApiResponse<IEnumerable<WorkUnitResponse>>(0, "Thành công", result));
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<string>(1, "Lỗi hệ thống: " + ex.Message, null));
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] WorkProcessCreateRequest request)
    {
        try
        {
            var result = await _workProcessService.CreateAsync(request);
            return Ok(new ApiResponse<bool>(0, "Tạo quá trình làm việc thành công", result));
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

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] WorkProcessUpdateRequest request)
    {
        try
        {
            var result = await _workProcessService.UpdateAsync(request);
            return Ok(new ApiResponse<bool>(0, "Cập nhật quá trình làm việc thành công", result));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new ApiResponse<string>(1, ex.Message, null));
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
    public async Task<IActionResult> Delete([FromBody] WorkProcessDeleteRequest request)
    {
        try
        {
            var result = await _workProcessService.DeleteAsync(request);
            return Ok(new ApiResponse<bool>(0, "Xóa quá trình làm việc thành công", result));
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