namespace Project_LMS.DTOs.Response;

public class DisciplineResponse
{
    public int Id { get; set; }
    public string? DisciplineContent { get; set; }
    //còn thiếu quyết định kỷ luật response này dựa vào figma
    public DateTime? CreateAt { get; set; }
}