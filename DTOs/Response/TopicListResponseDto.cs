using Project_LMS.DTOs.Response;

namespace Namespace.DTOs.Response.TopicListResponseDto; // Replace with your actual namespace

public class TopicListResponseDto
{
    // Thông tin header
    public string TeacherName { get; set; }
    public string ClassName { get; set; }
    public string SubjectName { get; set; }
    
    // Danh sách các items (được phân trang)
    public PaginatedResponse<TopicItemDto> TopicItems { get; set; }
}

// DTOs/Response/TopicItemDto.cs
public class TopicItemDto
{
    public int TopicId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTimeOffset? CloseAt { get; set; }
}
