using System.Text.RegularExpressions;

namespace Project_LMS.Helpers;

public static class IntValidator
{

    public static bool IsValid(string input) {
        if (string.IsNullOrWhiteSpace(input)) {
            return false;
        }
        return int.TryParse(input, out _);
    }
        
}