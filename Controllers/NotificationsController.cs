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

    public NotificationsController(INotificationsService notificationsService)
    {
        _notificationsService = notificationsService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetNotifications(int userId)
    {
        var notifications = await _notificationsService.GetNotificationsByUserIdAsync(userId);
        if (notifications.Status == 1)
        {
            return BadRequest(notifications);
        }

        return Ok(notifications);
    }

    [HttpGet("by-teaching-assignment")]
    public async Task<IActionResult> GetNotificationsByTeachingAssignment([FromQuery] int userId,
        [FromQuery] int teachingAssignmentId)
    {
        var notifications =
            await _notificationsService.GetNotificationsByUserAndTeachingAssignmentAsync(userId,
                teachingAssignmentId);
        if (notifications.Status == 1)
        {
            return BadRequest(notifications);
        }

        return Ok(notifications);
    }

    [HttpGet("all/{userId}")]
    public async Task<IActionResult> GetAllNotifications(int userId)
    {
        var notifications = await _notificationsService.GetAllNotificationsByUserAsync(userId);
        if (notifications.Status == 1)
        {
            return BadRequest(notifications);
        }

        return Ok(notifications);
    }

    [HttpPost("send-manual")]
    public async Task<IActionResult> SendMessageNotification(AddManualNotificationRequest request)
    {
        var notifications = await _notificationsService.AddManualNotificationAsync(request);
        if (notifications.Status == 1)
        {
            return BadRequest(notifications);
        }

        return Ok(notifications);
    }

    [HttpDelete("delete-multiple/user/{userId}")]
    public async Task<IActionResult> DeleteNotifications([FromBody] DeleteRequest request, int userId)
    {
        var response = await _notificationsService.DeleteNotificationAsync(request, userId);
        if (response.Status == 0)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [HttpPut("mark-read-multiple/user/{userId}")]
    public async Task<IActionResult> MarkNotificationsAsRead([FromBody] DeleteRequest request, int userId)
    {
        var response = await _notificationsService.SelectIsReadAsync(request, userId);
        if (response.Status == 0)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }
}