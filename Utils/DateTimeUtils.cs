namespace Project_LMS.Utils
{
    public static class DateTimeUtils
    {
        public static DateTime ToUnspecifiedKind(DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
        }
    }
}
