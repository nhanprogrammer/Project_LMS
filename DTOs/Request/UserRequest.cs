namespace Project_LMS.DTOs.Request
{
    public class UserRequest
    {
        public int? ConfigurationId { get; set; }
        public int? Role { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public bool? Active { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
    }
}
