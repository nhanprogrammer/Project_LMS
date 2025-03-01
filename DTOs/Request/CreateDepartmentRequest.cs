namespace Project_LMS.DTOs.Request;

public class CreateDepartmentRequest
{
    public string Name { get; set; }
    public DateTime? CreateAt { get; set; }
    public int? UserUpdate { get; set; }
}