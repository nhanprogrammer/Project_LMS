namespace Project_LMS.Helpers;

public static class EnumValidator
{
    public static bool IsValidEnumValue<T>(int value) where T : Enum
    {
        return Enum.IsDefined(typeof(T), value);
    }

    public static bool IsValidEnumValue<T>(string value) where T : Enum
    {
        return Enum.TryParse(typeof(T), value, true, out _);
    }
}
