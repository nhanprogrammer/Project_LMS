using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request;
public class GroupPermissionUpdateRequest
{
    [Required(ErrorMessage = "ID nhóm quyền không được để trống.")]
    [Range(1, int.MaxValue, ErrorMessage = "ID nhóm quyền không hợp lệ.")]
    public int GroupRoleId { get; set; }
    [Required(ErrorMessage = "Tên nhóm quyền không được để trống.")]
    public string GroupRoleName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool AllPermission { get; set; } = false;
    public List<ModulePermissionRequest> Permissions { get; set; }
}