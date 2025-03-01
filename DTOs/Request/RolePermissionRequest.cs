using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class RolePermissionRequest
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "RoleId is required")]
        public int? RoleId { get; set; }
        [Required(ErrorMessage = "ModuleId is required")]
        public int? ModuleId { get; set; }
        [Required(ErrorMessage = "PermissionId is required")]
        public int? PermissionId { get; set; }
    }
}