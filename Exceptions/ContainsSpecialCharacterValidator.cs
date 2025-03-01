using System.Text.RegularExpressions;

namespace Project_LMS.Helpers;

public static class ContainsSpecialCharacterValidator
{
    private static readonly Regex ContainsSpecialCharacterRegex = new(@"[^a-zA-Z0-9\s]", RegexOptions.Compiled);

    public static bool IsValid(string input) 
        => !string.IsNullOrWhiteSpace(input) && ContainsSpecialCharacterRegex.IsMatch(input);
}