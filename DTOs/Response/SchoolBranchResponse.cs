namespace Project_LMS.DTOs.Response
{
    public class SchoolBranchResponse
    {
        public int Id { get; set; }
        public int SchoolId { get; set; }
        public string BranchName { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string SchoolPhone { get; set; } = null!;
        public string Manager { get; set; } = null!;
        public string? Image { get; set; }

    }
}