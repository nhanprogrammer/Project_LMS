using System.Text.RegularExpressions;

namespace Project_LMS.Helpers;

public static class EmailValidator
{
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    // Kiểm tra email có hợp lệ không
    public static bool IsValid(string email) 
        => !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
}