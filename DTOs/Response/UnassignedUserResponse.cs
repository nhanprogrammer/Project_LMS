
namespace Project_LMS.DTOs.Response
{
    public class UnassignedUserResponse
    {
        public int Id { get; set; }          // ID người dùng
        public string? FullName { get; set; } // Họ và tên
        public string? UserCode { get; set; } // Mã người dùng
        public string? Email { get; set; }    // Email
    }
}