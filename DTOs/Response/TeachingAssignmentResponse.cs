namespace Project_LMS.DTOs.Response
{
    public class TeachingAssignmentResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? ClassId { get; set; }
        public string? ClassName { get; set; }
        public int? SubjectId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
    }
    public class TeachingAssignmentResponseCreateUpdate
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? FullName { get; set; } // Giảng viên
        public int? ClassId { get; set; }
        public string? ClassName { get; set; } // Lớp học
        public int? SubjectId { get; set; }
        public string? SubjectName { get; set; } // Môn học
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string? Description { get; set; } 
    }

    public class TeachingAssignmentResponseUpdateResponse
    {
        public int? UserId { get; set; }
        public int? ClassId { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string? Description { get; set; }
    }

    public class TeachingAssignmentWrapperResponse
    {
        public PaginatedResponse<TeachingAssignmentResponseCreateUpdate>? TeachingAssignments { get; set; }
    }

    public class UserResponseTeachingAssignment
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
    }

    public class TopicResponseByAssignmentId
    {
        public int TopicId { get; set; }
        public int? TeachingAssignmentId { get; set; }
        public int? UserId { get; set; }
        public string? FullName { get; set; }
        public string? ClassName { get; set; }
        public string? SubjectName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTimeOffset? CloseAt { get; set; }
    }

    public class ClassResponseSearch
    {
        public int Id { get; set; }
        public string? ClassName { get; set; }
    }
    public class SubjectResponseSearch
    {
        public int Id { get; set; }
        public string? SubjectName { get; set; }
    }
}
