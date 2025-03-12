namespace Project_LMS.DTOs.Request;

public class PermissionUserResponse
{
    public int? Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string GroupPermissionName { get; set; } = null!;
    public string Status { get; set; } = null!;
}