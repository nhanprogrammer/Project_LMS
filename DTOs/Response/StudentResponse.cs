namespace Project_LMS.DTOs.Response
{
    public class IdNamePair
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }

    public class StudentResponse
    {
        public int? StudentStatusId { get; set; }
        public IdNamePair? AcademicYear { get; set; } // Niên khóa: { Id, Name }
        public IdNamePair? Department { get; set; } // Khối: { Id, Name }
        public IdNamePair? Class { get; set; } // Lớp: { Id, Name }
        public string? UserCode { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public DateTime? StartDate { get; set; }
        public bool? Active { get; set; }
        public string? Image { get; set; }
        public bool? Gender { get; set; }
        public string? Ethnicity { get; set; }
        public string? Religion { get; set; }
        public string? PlaceOfBirth { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? StudyMode { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? ProvinceId { get; set; }
        public string? DistrictId { get; set; }
        public string? WardId { get; set; }
        public string? Alias { get; set; }
        public string? AdmissionType { get; set; }
        public string? National { get; set; }
        public string? FullnameFather { get; set; }
        public DateTime? BirthFather { get; set; }
        public string? WorkFather { get; set; }
        public string? PhoneFather { get; set; }
        public string? FullnameMother { get; set; }
        public DateTime? BirthMother { get; set; }
        public string? WorkMother { get; set; }
        public string? PhoneMother { get; set; }
        public string? FullnameGuardianship { get; set; }
        public DateTime? BirthGuardianship { get; set; }
        public string? WorkGuardianship { get; set; }
        public string? PhoneGuardianship { get; set; }
    }
}