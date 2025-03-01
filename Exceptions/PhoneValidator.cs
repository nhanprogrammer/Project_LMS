using System.Text.RegularExpressions;

namespace Project_LMS.Helpers
{
    public static class PhoneValidator
    {
        // Chấp nhận số điện thoại từ 9-15 chữ số, có thể có dấu "+" ở đầu
        private static readonly Regex PhoneRegex = new(@"^\+?\d{9,15}$", RegexOptions.Compiled);

        public static bool IsValid(string phone) 
            => !string.IsNullOrWhiteSpace(phone) && PhoneRegex.IsMatch(phone);
    }
}   