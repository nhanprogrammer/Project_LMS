namespace Project_LMS.DTOs.Request
{
    public class UpdateClassStudentOnlineRequest
    {
        public int Id { get; set; }
        public bool? IsMuted { get; set; }
        public bool? IsCamera { get; set; }
        public bool? IsAdmin { get; set; }
        public DateTime? LeaveTime { get; set; }
        public DateTime? UpdateAt { get; set; } = DateTime.UtcNow;
        public int UserUpdate { get; set; }
    }
}