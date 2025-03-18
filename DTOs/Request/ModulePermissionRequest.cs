namespace Project_LMS.DTOs.Request;
public class ModulePermissionRequest
{
    public int ModuleId { get; set; }
    public string? ModuleName { get; set; }
    public bool IsView { get; set; }
    public bool IsInsert { get; set; }
    public bool IsUpdate { get; set; }
    public bool IsDelete { get; set; }
    public bool EnterScore { get; set; }
}