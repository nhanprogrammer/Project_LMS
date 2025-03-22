namespace Project_LMS.DTOs.Response;

public class PermissionListGroupResponse
{
    public int? Id { get; set; }
    public string Name { get; set; } = null!;
    public string MemberCount { get; set; } = null!;
    public string Description { get; set; } = null!;
}