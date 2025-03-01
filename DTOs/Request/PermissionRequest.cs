namespace Project_LMS.DTOs.Request;

public class PermissionRequest
{
    public int? Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public bool? IsDelete { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UserCreate { get; set; }
    public int? UserUpdate { get; set; }
}