namespace Project_LMS.Helpers;

    /**
     * Lớp tiện ích TimeHelper cung cấp thời gian hiện tại theo múi giờ UTC+7 (ICT).
     * Điều này giúp đồng nhất thời gian cho toàn team bất kể múi giờ của từng máy.
     */
    public static class TimeHelper
    {
        /**
         * Thuộc tính Now lấy thời gian hiện tại theo múi giờ UTC+7 bằng cách cộng thêm 7 tiếng vào thời gian UTC hiện tại.
         * Lưu ý: Phương pháp này sử dụng offset cố định và không xử lý giờ mùa hè (daylight saving).
         */
        public static DateTime Now => DateTime.UtcNow.AddHours(7);

        /**
         * Thuộc tính NowUsingTimeZone lấy thời gian hiện tại theo múi giờ "SE Asia Standard Time" (ICT)
         * bằng cách chuyển đổi từ thời gian UTC sang múi giờ được cấu hình sẵn.
         * Phương pháp này sử dụng API TimeZoneInfo để đảm bảo chuyển đổi chính xác theo múi giờ đã được định nghĩa.
         */
        public static DateTime NowUsingTimeZone => TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow, 
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time")
        );
        
    }

