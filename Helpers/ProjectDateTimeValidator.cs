using System.ComponentModel.DataAnnotations;
using Project_LMS.DTOs.Request;

namespace Project_LMS.Helpers
{
    public class ProjectDateTimeValidator
    {
        public static ValidationResult ValidateFutureDate(DateTime? date, ValidationContext context)
        {
            if (date == null)
            {
                return ValidationResult.Success;
            }

            if (date.Value <= DateTime.Now)
            {
                return new ValidationResult("Ngày bắt đầu không thể nhỏ hơn ngày hiện tại");
            }

            return ValidationResult.Success;
        }

        public static ValidationResult ValidateEndDate(DateTime? endDate, ValidationContext context)
        {
            var instance = context.ObjectInstance as CreateLessonRequest;
            if (instance == null || endDate == null || instance.StartDate == null)
            {
                return ValidationResult.Success;
            }

            if (endDate.Value <= instance.StartDate.Value)
            {
                return new ValidationResult("Ngày kết thúc phải lớn hơn ngày bắt đầu");
            }

            return ValidationResult.Success;
        }
    }
}