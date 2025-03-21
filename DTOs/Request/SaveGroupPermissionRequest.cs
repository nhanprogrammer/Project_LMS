namespace Project_LMS.DTOs.Request;
public class SaveGroupPermissionRequest
{
    public int GroupRoleId { get; set; }
    public string GroupRoleName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool AllPermission { get; set; } = false;
    public List<ModulePermissionRequest> Permissions { get; set; }
}