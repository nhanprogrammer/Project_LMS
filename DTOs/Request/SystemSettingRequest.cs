namespace Project_LMS.DTOs.Request
{
    public class SystemSettingRequest
    {
        public bool? CaptchaEnabled { get; set; }
        public string? CurrentTheme { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
    }
}
