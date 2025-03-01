namespace Project_LMS.DTOs.Request;

public class UpdateModuleRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime? UpdateAt { get; set; }
    public int? UserUpdate { get; set; }
    public bool? IsDelete { get; set; }
}