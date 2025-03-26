using System.Text.Json.Serialization;
using FluentValidation;
using Project_LMS.Data;

namespace Project_LMS.DTOs.Request
{
    public class DependentRequest
    {
        public string? UserCode { get; set; }
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        [JsonIgnore]
        public int? UserCreate { get; set; }
        [JsonIgnore]
        public int? UserUpdate { get; set; }
    }
    public class DependentRequestValidator : AbstractValidator<DependentRequest>
    {
        private readonly ApplicationDbContext _context;
        public DependentRequestValidator(ApplicationDbContext context)
        {
            _context = context;
            RuleFor(d=>d.UserCode).NotNull().WithMessage("UserCode không được để trống.")
                .Must(UserExists).WithMessage("UserCode không tồn tại.");
            RuleFor(d => d.FullName).NotNull().WithMessage("FullName không được để trống.");
            RuleFor(d => d.Address).NotNull().WithMessage("Address không được để trống.");
            RuleFor(d => d.Phone).NotNull().WithMessage("Phone không được để trống.");
        }

        private bool UserExists(string? userCode)
        {
            if (userCode == null) return false; // Kiểm tra ID hợp lệ
            var user = _context.Users
                .FirstOrDefault(u => u.UserCode == userCode && u.IsDelete == false);
            return user != null;
        }
    }
}
