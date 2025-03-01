namespace Project_LMS.DTOs.Response
{
    public class RegistrationResponse
    {
        public int Id { get; set; }
        public int NationalityId { get; set; }
        public int SchoolId { get; set; }
        public int UserId { get; set; }
        public string Course { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public DateTime Birthday { get; set; }
        public bool Gender { get; set; }
        public string Education { get; set; } = null!;
        public string CurrentSchool { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Image { get; set; } = null!;
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }
    }
}