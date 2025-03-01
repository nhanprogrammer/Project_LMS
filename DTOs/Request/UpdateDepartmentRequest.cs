namespace Project_LMS.DTOs.Request;

public class UpdateDepartmentRequest
{
    public string Name { get; set; }
    public bool? IsDelete { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UserUpdate { get; set; }
}