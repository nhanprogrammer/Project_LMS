using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request;

public class UpdateDepartmentRequest
{
    [Required(ErrorMessage = "Tên không được để trống")]
    public string Name { get; set; }
        
    public bool? IsDelete { get; set; }
        
    public DateTime? UpdateAt { get; set; }
        
    // Người cập nhật: có thể là thông tin lấy từ context của hệ thống (nếu cần)
    public int? UserUpdate { get; set; }
}