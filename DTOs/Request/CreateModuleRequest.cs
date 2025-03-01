namespace Project_LMS.DTOs.Request;

public class CreateModuleRequest
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime? CreateAt { get; set; }
    public int? UserCreate { get; set; }
}
