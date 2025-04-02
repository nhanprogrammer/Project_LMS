using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Project_LMS.DTOs.Request
{
    public class StudentRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "StudentStatusId phải lớn hơn 0.")]
        public int? StudentStatusId { get; set; }

        [Required(ErrorMessage = "ClassId không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "ClassId phải lớn hơn 0.")]
        public int ClassId { get; set; }

        // UserCode không cần [Required] vì sẽ được tạo tự động nếu không cung cấp
        public string? UserCode { get; set; }

        [Required(ErrorMessage = "FullName không được để trống.")]
        [StringLength(100, ErrorMessage = "FullName không được dài quá 100 ký tự.")]
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [StringLength(100, ErrorMessage = "Email không được dài quá 100 ký tự.")]
        public string? Email { get; set; }

        [DataType(DataType.Date, ErrorMessage = "StartDate không hợp lệ.")]
        public DateTime? StartDate { get; set; }

        public string? Image { get; set; }

        public bool? Gender { get; set; }

        [StringLength(50, ErrorMessage = "Ethnicity không được dài quá 50 ký tự.")]
        public string? Ethnicity { get; set; }

        [StringLength(50, ErrorMessage = "Religion không được dài quá 50 ký tự.")]
        public string? Religion { get; set; }

        [StringLength(100, ErrorMessage = "PlaceOfBirth không được dài quá 100 ký tự.")]
        public string? PlaceOfBirth { get; set; }

        [DataType(DataType.Date, ErrorMessage = "BirthDate không hợp lệ.")]
        [CustomDateValidation(ErrorMessage = "BirthDate không thể là ngày trong tương lai.")]
        public DateTime? BirthDate { get; set; }

        [StringLength(50, ErrorMessage = "StudyMode không được dài quá 50 ký tự.")]
        public string? StudyMode { get; set; }

        [Required(ErrorMessage = "Phone không được để trống.")]
        [Phone(ErrorMessage = "Phone không hợp lệ.")]
        [RegularExpression(@"^(03[2-9]|05[6-9]|07[0-9]|08[1-9]|09[0-9]|02[0-9]|04[0-9])\d{7}$", 
            ErrorMessage = "Phone phải là số điện thoại Việt Nam hợp lệ (10 số, bắt đầu bằng 03, 05, 07, 08, 09, 02, 04).")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Address không được để trống.")]
        [StringLength(200, ErrorMessage = "Address không được dài quá 200 ký tự.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "ProvinceId không được để trống.")]
        [StringLength(10, ErrorMessage = "ProvinceId không được dài quá 10 ký tự.")]
        public string? ProvinceId { get; set; }

        [Required(ErrorMessage = "DistrictId không được để trống.")]
        [StringLength(10, ErrorMessage = "DistrictId không được dài quá 10 ký tự.")]
        public string? DistrictId { get; set; }

        [Required(ErrorMessage = "WardId không được để trống.")]
        [StringLength(10, ErrorMessage = "WardId không được dài quá 10 ký tự.")]
        public string? WardId { get; set; }

        [StringLength(50, ErrorMessage = "Alias không được dài quá 50 ký tự.")]
        public string? Alias { get; set; }

        [StringLength(50, ErrorMessage = "AdmissionType không được dài quá 50 ký tự.")]
        public string? AdmissionType { get; set; }

        [StringLength(50, ErrorMessage = "National không được dài quá 50 ký tự.")]
        public string? National { get; set; }

        [StringLength(100, ErrorMessage = "FullnameFather không được dài quá 100 ký tự.")]
        public string? FullnameFather { get; set; }

        [DataType(DataType.Date, ErrorMessage = "BirthFather không hợp lệ.")]
        [CustomDateValidation(ErrorMessage = "BirthFather không thể là ngày trong tương lai.")]
        public DateTime? BirthFather { get; set; }

        [StringLength(100, ErrorMessage = "WorkFather không được dài quá 100 ký tự.")]
        public string? WorkFather { get; set; }

        [Phone(ErrorMessage = "PhoneFather không hợp lệ.")]
        [RegularExpression(@"^(03[2-9]|05[6-9]|07[0-9]|08[1-9]|09[0-9]|02[0-9]|04[0-9])\d{7}$", 
            ErrorMessage = "PhoneFather phải là số điện thoại Việt Nam hợp lệ (10 số, bắt đầu bằng 03, 05, 07, 08, 09, 02, 04).")]
        public string? PhoneFather { get; set; }

        [StringLength(100, ErrorMessage = "FullnameMother không được dài quá 100 ký tự.")]
        public string? FullnameMother { get; set; }

        [DataType(DataType.Date, ErrorMessage = "BirthMother không hợp lệ.")]
        [CustomDateValidation(ErrorMessage = "BirthMother không thể là ngày trong tương lai.")]
        public DateTime? BirthMother { get; set; }

        [StringLength(100, ErrorMessage = "WorkMother không được dài quá 100 ký tự.")]
        public string? WorkMother { get; set; }

        [Phone(ErrorMessage = "PhoneMother không hợp lệ.")]
        [RegularExpression(@"^(03[2-9]|05[6-9]|07[0-9]|08[1-9]|09[0-9]|02[0-9]|04[0-9])\d{7}$", 
            ErrorMessage = "PhoneMother phải là số điện thoại Việt Nam hợp lệ (10 số, bắt đầu bằng 03, 05, 07, 08, 09, 02, 04).")]
        public string? PhoneMother { get; set; }

        [StringLength(100, ErrorMessage = "FullnameGuardianship không được dài quá 100 ký tự.")]
        public string? FullnameGuardianship { get; set; }

        [DataType(DataType.Date, ErrorMessage = "BirthGuardianship không hợp lệ.")]
        [CustomDateValidation(ErrorMessage = "BirthGuardianship không thể là ngày trong tương lai.")]
        public DateTime? BirthGuardianship { get; set; }

        [StringLength(100, ErrorMessage = "WorkGuardianship không được dài quá 100 ký tự.")]
        public string? WorkGuardianship { get; set; }

        [Phone(ErrorMessage = "PhoneGuardianship không hợp lệ.")]
        [RegularExpression(@"^(03[2-9]|05[6-9]|07[0-9]|08[1-9]|09[0-9]|02[0-9]|04[0-9])\d{7}$", 
            ErrorMessage = "PhoneGuardianship phải là số điện thoại Việt Nam hợp lệ (10 số, bắt đầu bằng 03, 05, 07, 08, 09, 02, 04).")]
        public string? PhoneGuardianship { get; set; }

        [JsonIgnore]
        public int? UserCreate { get; set; }

        [JsonIgnore]
        public int? UserUpdate { get; set; }
    }

    public class DeleteStudentRequest
    {
        [Required(ErrorMessage = "UserCode không được để trống.")]
        [MinLength(1, ErrorMessage = "Danh sách UserCode phải có ít nhất 1 mã.")]
        public List<string> UserCode { get; set; }
    }
}


public class CustomDateValidationAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success; // Cho phép null
        }

        DateTime date = (DateTime)value;
        if (date > DateTime.Now)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}