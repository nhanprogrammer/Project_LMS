namespace Project_LMS.DTOs.Request
{
    public class UpdateClassOnlineRequest
    {
        public int Id { get; set; }
        public string ClassTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ClassDescription { get; set; }
        public int MaxStudents { get; set; }
        public bool ClassStatus { get; set; }
        public string? ClassPassword { get; set; }
        public DateTime? UpdateAt { get; set; } = DateTime.UtcNow;
        public int UserUpdate { get; set; }
        public string ClassCode { get; internal set; }
        public int TeacherId { get; internal set; }
    }
}
