namespace Project_LMS.DTOs.Request
{

    public class CreateRoomRequest
    {
        public string? ClassCode { get; set; }
        public string? ClassName { get; set; }
        public int CreatorUserId { get; set; }
    }
}