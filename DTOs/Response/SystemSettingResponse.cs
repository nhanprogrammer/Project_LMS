namespace Project_LMS.DTOs.Response
{
    public class SystemSettingResponse
    {
        public int Id { get; set; }
        public bool? CaptchaEnabled { get; set; }
        public string? CurrentTheme { get; set; }
        public string? Language { get; set; }
        //public DateTime? CreateAt { get; set; }
        //public DateTime? UpdateAt { get; set; }
    }
    public class UserSystemSettingResponse
    {
        public int UserId { get; set; }
        public bool? CaptchaEnabled { get; set; }
        public string? CurrentTheme { get; set; }
        public string? Language { get; set; }
        //public DateTime? CreateAt { get; set; }
        //public DateTime? UpdateAt { get; set; }
    }
}
