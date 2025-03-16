using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request;

public class UpdateDepartmentRequest
{
    public int id { get; set; }

    [Required(ErrorMessage = "Tên không được để trống")]
    public string name { get; set; }

    public bool? isDelete { get; set; }

    public DateTime? updateAt { get; set; }

    // Người cập nhật: có thể là thông tin lấy từ context của hệ thống (nếu cần)
    public int? userUpdate { get; set; }
}