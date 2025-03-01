namespace Project_LMS.Helpers;

public static class ObjectValidator
{
    public static bool IsNotNull(object obj) => obj != null;

    public static bool IsValidObject<T>(T obj) => obj != null && typeof(T).IsClass;
}
