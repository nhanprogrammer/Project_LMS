namespace Project_LMS.DTOs.Request;

public class UpdateAcademicHoldRequest
{
        public int Id { get; set; }
        public int? UserId { get; set; }
        public DateTime HoldDate { get; set; }
        public int? HoldDuration { get; set; }
        public string? Reason { get; set; }
        public string? FileName { get; set; }
        public DateTime CreateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

}