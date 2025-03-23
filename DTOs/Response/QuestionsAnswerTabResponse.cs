namespace Project_LMS.DTOs.Response;

public class QuestionsAnswerTabResponse
{
    // Tổng số lượng (hiển thị trên tab, ví dụ: 10)
    public int TotalCount { get; set; }

    // Dữ liệu câu hỏi
    public List<QuestionsAnswerResponse> Questions { get; set; }

    // Dữ liệu topic
    public List<TopicResponse> Topics { get; set; }
}