using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class TeachingAssignmentRequest
    {
        [Required(ErrorMessage = "Class là bắt buộc.")]
        public int? ClassId { get; set; }

        [Required(ErrorMessage = "Subject là bắt buộc.")]
        public int? SubjectId { get; set; }

        [Required(ErrorMessage = "Ngày bắt đàu là bắt buộc.")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc.")]
        [CustomValidation(typeof(TeachingAssignmentRequest), nameof(ValidateEndDate))]
        public DateTime? EndDate { get; set; }

        public string? Description { get; set; }

        // Phương thức validation để kiểm tra EndDate >= StartDate
        public static ValidationResult ValidateEndDate(DateTime? endDate, ValidationContext context)
        {
            var instance = context.ObjectInstance as TeachingAssignmentRequest;
            if (instance == null || !instance.StartDate.HasValue || !endDate.HasValue)
            {
                return ValidationResult.Success!; // Sẽ được xử lý bởi [Required]
            }

            if (endDate.Value < instance.StartDate.Value)
            {
                return new ValidationResult($"Ngày kết thúc ({endDate.Value}) phải lớn hơn hoặc bằng ngày bắt đầu ({instance.StartDate.Value}).");
            }

            return ValidationResult.Success!;
        }
    }

    public class TeachingAssignmentRequestCreate
    {
        [Required(ErrorMessage = "User là bắt buộc.")]
        public int? UserId { get; set; }

        [Required(ErrorMessage = "Class là bắt buộc.")]
        public int? ClassId { get; set; }

        [Required(ErrorMessage = "Subject là bắt buộc.")]
        public int? SubjectId { get; set; }

        [Required(ErrorMessage = "StartDate là bắt buộc.")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "EndDate là bắt buộc.")]
        [CustomValidation(typeof(TeachingAssignmentRequestCreate), nameof(ValidateEndDate))]
        public DateTime? EndDate { get; set; }

        public string? Description { get; set; }

        // Phương thức validation để kiểm tra EndDate >= StartDate
        public static ValidationResult ValidateEndDate(DateTime? endDate, ValidationContext context)
        {
            var instance = context.ObjectInstance as TeachingAssignmentRequestCreate;
            if (instance == null || !instance.StartDate.HasValue || !endDate.HasValue)
            {
                return ValidationResult.Success!; // Sẽ được xử lý bởi [Required]
            }

            if (endDate.Value < instance.StartDate.Value)
            {
                return new ValidationResult($"Ngày kết thúc ({endDate.Value}) phải lớn hơn hoặc bằng ngày bắt đầu ({instance.StartDate.Value}).");
            }

            return ValidationResult.Success!;
        }
    }

    public class TeachingAssignmentRequestUpdate
    {
        [Required(ErrorMessage = "TeachingAssignment là bắt buộc.")]
        [Range(1, int.MaxValue, ErrorMessage = "TeachingAssignmentId phải lớn hơn 0.")]
        public int teachingAssignmentId { get; set; }

        [Required(ErrorMessage = "Class là bắt buộc.")]
        public int? ClassId { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc là bắt buộc.")]
        [CustomValidation(typeof(TeachingAssignmentRequestUpdate), nameof(ValidateEndDate))]
        public DateTime? EndDate { get; set; }

        public string? Description { get; set; }

        // Phương thức validation để kiểm tra EndDate >= StartDate
        public static ValidationResult ValidateEndDate(DateTime? endDate, ValidationContext context)
        {
            var instance = context.ObjectInstance as TeachingAssignmentRequestUpdate;
            if (instance == null || !instance.StartDate.HasValue || !endDate.HasValue)
            {
                return ValidationResult.Success!; // Sẽ được xử lý bởi [Required]
            }

            if (endDate.Value < instance.StartDate.Value)
            {
               return new ValidationResult($"Ngày kết thúc ({endDate.Value}) phải lớn hơn hoặc bằng ngày bắt đầu ({instance.StartDate.Value}).");
            }

            return ValidationResult.Success!;
        }
    }
}