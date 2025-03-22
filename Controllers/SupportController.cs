using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers;
[ApiController]
[Route("api/[controller]")]
public class SupportController : ControllerBase
{
    private readonly ISupportService _supportService;

    public SupportController(ISupportService supportService)
    {
        _supportService = supportService;
    }
    
    [HttpPost("send")]
    public async Task<IActionResult> SendEmail([FromBody] SupportRequest supportRequest)
    {
        try
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            await _supportService.SendEmailAsync(supportRequest.Subject, supportRequest.HtmlBody, token);
            return Ok("Email đã được gửi thành công.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Lỗi khi gửi email: {ex.Message}");
        }
    }
    
}