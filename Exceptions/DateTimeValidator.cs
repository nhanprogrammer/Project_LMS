namespace Project_LMS.Helpers
{
    public static class DateTimeValidator
    {
        public static bool IsValidDateTime(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return DateTime.TryParse(input, out _);
        }
        public static bool IsFutureDate(DateTime date) 
            => date > DateTime.UtcNow; 

        public static bool IsPastDate(DateTime date) 
            => date < DateTime.UtcNow;

        public static bool IsToday(DateTime date) 
            => date.Date == DateTime.UtcNow.Date;

        public static bool IsWithinRange(DateTime date, DateTime start, DateTime end)
        {
            if (start > end)
                throw new ArgumentException("Ngày bắt đầu không thể lớn hơn ngày kết thúc.");

            return date >= start && date <= end;
        }
    }
}