namespace Project_LMS.DTOs.Request;

public class CreateDisciplineRequest
{
    public int StudentId { get; set; }
    public int SemesterId { get; set; }
    public int DisciplineCode { get; set; }
    public string Name { get; set; } 
    public string? DisciplineContent { get; set; }
    public DateTime? CreateAt { get; set; }
    public int? UserCreate { get; set; }
}
