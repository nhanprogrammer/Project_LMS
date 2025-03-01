namespace Project_LMS.DTOs.Request
{
    public class CreateClassOnlineRequest
    {
        public int TeacherId { get; set; }
        public string ClassCode { get; set; }
        public string ClassTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ClassDescription { get; set; }
        public int MaxStudents { get; set; }
        public bool ClassStatus { get; set; }
        public string ClassLink { get; set; }
        public string? ClassPassword { get; set; }
        public DateTime? CreateAt { get; set; } = DateTime.UtcNow;
        public int UserCreate { get; set; }
    }
}


