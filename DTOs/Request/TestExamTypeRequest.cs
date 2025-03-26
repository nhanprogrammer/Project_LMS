using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class TestExamTypeRequest : IValidatableObject
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Tên loại điểm là bắt buộc.")]
        [StringLength(255, ErrorMessage = "Tên loại điểm không được vượt quá 255 ký tự.")]
        public string? PointTypeName { get; set; }

        [Required(ErrorMessage = "Hệ số là bắt buộc.")]
        [Range(1, 3, ErrorMessage = "Hệ số phải nằm trong khoảng từ 1 đến 3.")]
        public int? Coefficient { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số cột điểm tối thiểu kỳ 1 phải là số dương.")]
        public int? MinimunEntriesSem1 { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Số cột điểm tối thiểu kỳ 2 phải là số dương.")]
        public int? MinimunEntriesSem2 { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!MinimunEntriesSem1.HasValue && !MinimunEntriesSem2.HasValue)
            {
                yield return new ValidationResult(
                    "Phải có ít nhất một trong hai trường: Số cột điểm tối thiểu kỳ 1 hoặc kỳ 2.",
                    new[] { nameof(MinimunEntriesSem1), nameof(MinimunEntriesSem2) }
                );
            }
        }
    }
}