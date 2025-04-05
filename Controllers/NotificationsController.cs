using Microsoft.AspNetCore.Mvc;
using Project_LMS.DTOs.Request;
using Project_LMS.DTOs.Response;
using Project_LMS.Interfaces;
using Project_LMS.Interfaces.Services;

namespace Project_LMS.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly INotificationsService _notificationsService;
    private readonly IAuthService _authService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationsService notificationsService, IAuthService authService,
        ILogger<NotificationsController> logger)
    {
        _notificationsService = notificationsService;
        _authService = authService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications()
    {
        var user = await _authService.GetUserAsync();
        var notifications = await _notificationsService.GetNotificationsByUserIdAsync(user.Id);
        if (notifications.Status == 1)
        {
            return BadRequest(notifications);
        }

        return Ok(notifications);
    }

    [HttpGet("by-teaching-assignment")]
    public async Task<IActionResult> GetNotificationsByTeachingAssignment(
        [FromQuery] int teachingAssignmentId)
    {
        var user = await _authService.GetUserAsync();
        var notifications =
            await _notificationsService.GetNotificationsByUserAndTeachingAssignmentAsync(user.Id,
                teachingAssignmentId);
        if (notifications.Status == 1)
        {
            return BadRequest(notifications);
        }

        return Ok(notifications);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllNotifications()
    {
        var user = await _authService.GetUserAsync();
        var notifications = await _notificationsService.GetAllNotificationsByUserAsync(user.Id);
        if (notifications.Status == 1)
        {
            return BadRequest(notifications);
        }

        return Ok(notifications);
    }

    [HttpPost("send-manual")]
    public async Task<IActionResult> SendMessageNotification(AddManualNotificationRequest request)
    {
        var user = await _authService.GetUserAsync();
        request.SenderId = user.Id;
        var notifications = await _notificationsService.AddManualNotificationAsync(request);
        if (notifications.Status == 1)
        {
            return BadRequest(notifications);
        }

        return Ok(notifications);
    }

    [HttpDelete("delete-multiple/user")]
    public async Task<IActionResult> DeleteNotifications([FromBody] DeleteRequest request)
    {
        _logger.LogInformation("Bắt đầu xử lý DeleteNotifications với request: {@Request}", request);
        var user = await _authService.GetUserAsync();
        if (user == null)
        {
            _logger.LogWarning("Không tìm thấy user từ _authService.GetUserAsync()");
            return Unauthorized(new ApiResponse<bool>(1, "Không tìm thấy user!", false));
        }

        var response = await _notificationsService.DeleteNotificationAsync(request, user.Id);
        if (response.Status == 0)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPut("mark-read-multiple/user")]
    public async Task<IActionResult> MarkNotificationsAsRead([FromBody] DeleteRequest request)
    {
        _logger.LogInformation("Bắt đầu xử lý MarkNotificationsAsRead với request: {@Request}", request);
        var user = await _authService.GetUserAsync();
        if (user == null)
        {
            _logger.LogWarning("Không tìm thấy user từ _authService.GetUserAsync()");
            return Unauthorized(new ApiResponse<bool>(1, "Không tìm thấy user!", false));
        }

        var response = await _notificationsService.SelectIsReadAsync(request, user.Id);
        if (response.Status == 0)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

}