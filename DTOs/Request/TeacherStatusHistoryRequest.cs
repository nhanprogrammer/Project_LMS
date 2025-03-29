using FluentValidation;
using Project_LMS.Data;
using Project_LMS.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Project_LMS.DTOs.Request
{
    public class TeacherStatusHistoryRequest
    {
        public string? UserCode { get; set; }
        public string? Note { get; set; }
        public DateTime? LeaveDate { get; set; }
        public string? FileName { get; set; }
        [JsonIgnore]
        public DateTime? CreatedAt { get; set; }
        [JsonIgnore]
        public DateTime? UpdatedAt { get; set; }
    }

    public class TeacherStatusHistoryRequestValidator : AbstractValidator<TeacherStatusHistoryRequest>
    {
        private readonly ApplicationDbContext _context;
        public TeacherStatusHistoryRequestValidator(ApplicationDbContext context)
        {
            _context = context;
            RuleFor(t=>t.UserCode).NotNull().WithMessage("UserCode không được để trống.")
                .Must(UserExists).WithMessage("UserCode không tồn tại.");
            RuleFor(t => t.Note).NotNull().WithMessage("Note không được để trống.");

            RuleFor(t=>t.LeaveDate).NotNull().WithMessage("LeaveDate không được để trống.")
                .Must(t=>t >= DateTime.Today).WithMessage("Ngày không được nhỏ hơn ngày hiện tại.");
        }
        private bool UserExists(string? userCode)
        {
            return (_context.Users
                .Any(u => u.UserCode.Equals(userCode) && u.IsDelete == false && u.Role.Name.ToLower().Contains("teacher")));
        }
    }
}
