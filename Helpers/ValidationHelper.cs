using Project_LMS.Exceptions;

namespace Project_LMS.Helpers;

public static class ValidationHelper
{
    public static void ThrowIfFalse(bool condition, string errorMessage)
    {
        if (!condition)
            throw new ValidationException(errorMessage);
    }

    public static void ThrowIfNull(object obj, string errorMessage)
    {
        if (obj == null)
            throw new ValidationException(errorMessage);
    }
}
