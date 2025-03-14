namespace Project_LMS.Helpers;

public static class StringValidator
{
    public static bool ContainsSpecialCharacters(string input)
    => input.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c) && c != ',');

    public static bool IsOnlyLettersAndNumbers(string input)
    => input.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));

    public static bool ConvertToString(object input)
    {
        try
        {
            input?.ToString();
            return true;
            
        }
        catch (Exception)
        {
            return false;
        }
    }
}
