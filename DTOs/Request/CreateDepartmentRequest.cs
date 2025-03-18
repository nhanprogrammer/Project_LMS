using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request;

public class CreateDepartmentRequest
{
    [Required(ErrorMessage = "Mã phòng ban không được để trống")]
    public string DepartmentCode { get; set; }
        
    [Required(ErrorMessage = "Tên không được để trống")]
    public string Name { get; set; }
        
    // Có thể truyền vào ngày tạo, hoặc hệ thống tự gán
    public DateTime? CreateAt { get; set; }
        
    // Foreign key: UserId liên kết với User
    [Required(ErrorMessage = "UserId không được để trống")]
    public int UserId { get; set; }
}