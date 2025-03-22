namespace Project_LMS.DTOs.Response;

public class TeacherTestExamDetailResponse
{
    public int ExamNumber { get; set; }
    public string StartDate { get; set; }

    public string SubjectName { get; set; }
    public string ClassList { get; set; }
    public string Duration { get; set; }
    public string Description { get; set; }
    public string Attachment { get; set; }
    public bool IsAttachmentRequired { get; set; }
    public int Quantity { get; set; }
    public List<RelatedTestDTO> RelatedTests { get; set; }
}

public class RelatedTestDTO
{
    public int ExamNumber { get; set; }
    public string StartDate { get; set; }
}