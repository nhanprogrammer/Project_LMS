namespace Project_LMS.DTOs.Response;

public class ClassMemberResponse
{
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string? Avatar { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Role { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; }
    public int Views { get; set; }

    // Thay đổi từ List<int> thành List<QuestionInfo>
    public List<QuestionInfo> Questions { get; set; } = new List<QuestionInfo>();

    // Thay đổi từ List<int> thành List<AnswerInfo>
    public List<AnswerInfo> Answers { get; set; } = new List<AnswerInfo>();
    public int TotalQuestions { get; set; } // Số lượng câu hỏi
    public int TotalAnswers { get; set; }   // Số lượng câu trả lời
}

public class QuestionInfo
{
    public int Id { get; set; }
    public string Message { get; set; }
    public DateTime? CreateAt { get; set; }
    public string? Avatar { get; set; }
    public string? FullName { get; set; } // Thêm thuộc tính FullName
}

public class AnswerInfo
{
    public int Id { get; set; }
    public string Message { get; set; }
    public DateTime? CreateAt { get; set; }
    public string? Avatar { get; set; }
    public string? FullName { get; set; } // Thêm thuộc tính FullName
}