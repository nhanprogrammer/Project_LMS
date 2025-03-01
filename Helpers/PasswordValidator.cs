using System.Text.RegularExpressions;

namespace Project_LMS.Helpers
{
    public static class PasswordValidator
    {
        // Kiểm tra mật khẩu có hợp lệ không (tối thiểu 8 ký tự, ít nhất 1 chữ hoa, 1 chữ thường, 1 số, 1 ký tự đặc biệt)
        private static readonly Regex PasswordRegex = new(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", RegexOptions.Compiled);

        public static bool IsValid(string password) 
            => !string.IsNullOrWhiteSpace(password) && PasswordRegex.IsMatch(password);
    }
}