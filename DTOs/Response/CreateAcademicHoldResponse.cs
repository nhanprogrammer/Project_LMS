namespace Project_LMS.DTOs.Response
{
    public class CreateAcademicHoldResponse
    {
        public int Id { get; set; } 
        public int ClassId { get; set; }
        public int? UserId { get; set; }
        public DateTimeOffset HoldDate { get; set; }
        public int? HoldDuration { get; set; }
        public string? Reason { get; set; }
        public string? FileName { get; set; }
    }
}
