using System.Collections;

namespace Project_LMS.DTOs.Response
{
    public class User_AcademicHoldResponse
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public BitArray? Gender { get; set; }
        public DateTimeOffset? BirthDate { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Ethnicity { get; set; }
        public string? Religion { get; set; }
        public string? AcademicEndDate { get; set; }
        public string? AcademicStartDate { get; set; }
        public string? DepartmentName { get; set; } 
        public string? ClassName { get; set; } 
        public string? UserCode { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public string? AdmissionType { get; set; }
        public string? StatusName_Student { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? FullnameFather { get; set; }
        public string? FullnameMother { get; set; }
        public string? FullnameGuardianship { get; set; }
        public DateTimeOffset? BirthFather { get; set; }
        public DateTimeOffset? BirthMother { get; set; }
        public DateTimeOffset? BirthGuardianship { get; set; }
        public string? WorkFather { get; set; }
        public string? WorkMother { get; set; }
        public string? WorkGuardianship { get; set; }
        public string? PhoneFather { get; set; }
        public string? PhoneMother { get; set; }
        public string? PhoneGuardianship { get; set; }
    }

}

