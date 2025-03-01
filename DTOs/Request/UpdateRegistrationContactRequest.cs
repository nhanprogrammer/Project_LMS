namespace Project_LMS.DTOs.Request
{
    public class UpdateRegistrationContactRequest
    {
        public int Id { get; set; }
        public int RegistrationId { get; set; }
        public string FamilyName { get; set; } = null!;
        public string FamilyAddress { get; set; } = null!;
        public string FamilyNumber { get; set; } = null!;
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }
    }
}
