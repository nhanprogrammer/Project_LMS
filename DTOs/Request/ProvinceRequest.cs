using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request;

public class PermissionIdRequest
{
    [Required(ErrorMessage = "Id là bắt buộc.")]
    public int Id { get; set; }

}