public class TeacherTestExamRequest
{
    public int Id { get; set; }
    public int SubjectId { get; set; }
    public int? DepartmentId { get; set; }
    public string Topic { get; set; }
    public string Form { get; set; }
    public bool SelectALL { get; set; }
    public string Duration { get; set; }  // Sử dụng string thay vì TimeOnly
    public int TestExamType { get; set; }
    public DateTimeOffset StartDate { get; set; } 
    public DateTimeOffset EndDate { get; set; }   
    public string Description { get; set; }
    public string Attachment { get; set; }
    public bool IsAttachmentRequired { get; set; }
    public List<int>? classIds { get; set; }
    
    
    public bool IsDoc { get; set; } = false; // Defaults to false
    public bool IsPowerpoint { get; set; } = false;
    public bool IsXxls { get; set; } = false;
    public bool IsJpeg { get; set; } = false;
    public bool Is10 { get; set; } = false;
    public bool Is20 { get; set; } = false;
    public bool Is30 { get; set; } = false;
    public bool Is40 { get; set; } = false;
}