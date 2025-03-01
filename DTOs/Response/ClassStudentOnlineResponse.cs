namespace Project_LMS.DTOs.Response
{
    public class ClassStudentOnlineResponse
    {
        public int Id { get; set; }
        public int ClassId { get; set; }
        public string StudentName { get; set; }
        public bool? IsMuted { get; set; }
        public bool? IsCamera { get; set; }
        public bool? IsAdmin { get; set; }
        public DateTime? JoinTime { get; set; }
        public DateTime? LeaveTime { get; set; }
    }
}