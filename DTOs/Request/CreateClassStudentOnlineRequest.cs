namespace Project_LMS.DTOs.Request;

public class CreateClassStudentOnlineRequest
{
    public int ClassId { get; set; }
    public int StudentId { get; set; }
    public bool IsMuted { get; set; }
    public bool IsCamera { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime JoinTime { get; set; } = DateTime.UtcNow;
    public DateTime? LeaveTime { get; set; }
    public DateTime? CreateAt { get; set; } = DateTime.UtcNow;
    public int UserCreate { get; set; }
}
