namespace Project_LMS.Helpers
{
    public static class NumberValidator
    {
        public static bool IsInRange(decimal value, decimal min, decimal max) 
            => value >= min && value <= max;

        public static bool IsNonNegative(decimal value) 
            => value >= 0;

        public static bool IsInteger(object value) 
            => int.TryParse(value?.ToString(), out _);
        
    }
}