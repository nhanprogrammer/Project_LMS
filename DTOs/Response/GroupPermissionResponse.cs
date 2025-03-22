using Project_LMS.DTOs.Request;
namespace Project_LMS.DTOs.Response;
public class GroupPermissionResponse
{
    public int GroupRoleId { get; set; }
    public string GroupRoleName { get; set; }
    public string Description { get; set; }
    public List<ModulePermissionRequest> Permissions { get; set; }
}

