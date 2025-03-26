namespace Project_LMS.DTOs.Response
{
    public class UserInRoomResponse
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Image { get; set; }
        public string? RoleName { get; set; }
        public bool IsMuted { get; set; }
        public bool IsCamera { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime? JoinTime { get; set; }
    }
}