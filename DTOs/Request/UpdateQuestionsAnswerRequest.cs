namespace Project_LMS.DTOs.Request
{
    public class UpdateQuestionsAnswerRequest
    {
        public int Id { get; set; } // Định danh của câu hỏi hoặc câu trả lời cần cập nhật

        public int? UserUpdate { get; set; } // Định danh của người dùng thực hiện cập nhật (bắt buộc)

        public string? Message { get; set; } // Nội dung mới của câu hỏi hoặc câu trả lời (tùy chọn)

        public string? FileName { get; set; } // Tên file hoặc dữ liệu file mới (tùy chọn)

        public int? TeachingAssignmentId { get; set; } // Phân công giảng dạy mới (tùy chọn, dùng để cập nhật TeachingAssignmentId)
    }
}