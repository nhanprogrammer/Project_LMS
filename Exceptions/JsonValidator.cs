namespace Project_LMS.Helpers;

using System.Text.Json;

public static class JsonValidator
{
   public static bool IsValidJson(string jsonString)
{
    if (string.IsNullOrWhiteSpace(jsonString))
    {
        return false;
    }

    try
    {
        using (JsonDocument.Parse(jsonString))
        {
            return true;
        }
    }
    catch
    {
        return false;
    }
}

}