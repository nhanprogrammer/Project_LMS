namespace Project_LMS.DTOs.Request;

public class UpdateDisciplineRequest
{
    public int StudentId { get; set; }
    public int SemesterId { get; set; }
    public int DisciplineCode { get; set; }
    public string Name { get; set; } = null!;
    public string? DisciplineContent { get; set; }
    public bool? IsDelete { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UserUpdate { get; set; }
}
