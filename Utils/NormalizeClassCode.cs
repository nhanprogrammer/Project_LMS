using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

public static class StringHelper
{
    public static string NormalizeClassCode(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            return string.Empty;

        // Bỏ dấu tiếng Việt
        string normalizedString = className.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();

        foreach (char c in normalizedString)
        {
            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }
        string noDiacritics = sb.ToString().Normalize(NormalizationForm.FormC);

        // Xóa ký tự không phải chữ cái hoặc số
        string cleanedString = Regex.Replace(noDiacritics, @"[^a-zA-Z0-9]", "");

        // Chuyển thành chữ in hoa
        return cleanedString.ToUpper();
    }
}
