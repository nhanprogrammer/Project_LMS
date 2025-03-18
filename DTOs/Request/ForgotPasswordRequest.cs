namespace Project_LMS.DTOs.Request
{
    public class ForgotPasswordRequest
    {
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Confirm { get; set; }
    }
}
