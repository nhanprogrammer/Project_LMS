namespace Project_LMS.DTOs.Response
{
    public class UserResponse
    {
        public int Id { get; set; }
        public int? ConfigurationId { get; set; }
        public int? Role { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public bool? Active { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
    }
}
