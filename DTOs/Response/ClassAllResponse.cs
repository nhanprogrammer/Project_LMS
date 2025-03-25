namespace Project_LMS.DTOs.Response
{
    public class ClassLessonResponse
    {
        public int Id { get; set; }
        public string? ClassLessonCode { get; set; }
        public string? Description { get; set; }
        public string? PaswordLeassons { get; set; }
        public string? Topic { get; set; }
        public string? Duration { get; set; }
        public string? LessonLink { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsResearchable { get; set; }
        public bool? IsAutoStart { get; set; }
        public bool? IsSave { get; set; }
        public string TeacherName { get; set; }
    }

    public class ClassAllResponse
    {
        // Existing properties
        public int Id { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int? ClassTypeId { get; set; }
        public string? ClassTypeName { get; set; }
        public int? AcademicYearId { get; set; }
        public string? AcademicYearName { get; set; }
        public int? UserId { get; set; }
        public string? TeacherName { get; set; }
        public string? Description { get; set; }
        public string? ClassLink { get; set; }
        public string? Name { get; set; }
        public int? StudentCount { get; set; }
        public string? ClassCode { get; set; }
        public string? PasswordClass { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StatusClass { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDelete { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        // New property for lessons
        public List<ClassLessonResponse> Lessons { get; set; } = new List<ClassLessonResponse>();
    }
}