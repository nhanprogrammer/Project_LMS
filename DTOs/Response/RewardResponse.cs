namespace Project_LMS.DTOs.Response
{
    public class RewardResponse
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int SemesterId { get; set; }
        public int RewardCode { get; set; }
        public string Name { get; set; } = null!;
        public string? RewardContent { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
    }
}