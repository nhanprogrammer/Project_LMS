namespace Project_LMS.DTOs.Response
{
    public class SchoolTransferResponse
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? SchoolBranchesId { get; set; }
        public DateTime? TransferDate { get; set; }
        public string? Semester { get; set; }
        public string? Reason { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
    }

}