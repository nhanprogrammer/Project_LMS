namespace Project_LMS.DTOs.Response
{
    public class TopicResponse
    {
        public int Id { get; set; }
        public int? TeachingAssignmentId { get; set; }
        public int? UserId { get; set; }
        public string? Avatar { get; set; } // Sửa từ Avartar (int?) thành Avatar (string?)
        public string? FullName { get; set; } // Thêm FullName
        public int? TopicId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }
        public string? Title { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? CloseAt { get; set; }
        public int Views { get; set; } // Số lượt xem
        public int Replies { get; set; } // Số lượt trả lời
        public string? RoleName { get; set; }
        public List<TopicResponse> Comments { get; set; }
    }
}