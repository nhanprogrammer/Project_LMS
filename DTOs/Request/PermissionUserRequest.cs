namespace Project_LMS.DTOs.Request;

public class PermissionUserRequest
{
    public int UserId { get; set; }
    public int GroupId { get; set; }
    public bool Disable { get; set; }
    
}