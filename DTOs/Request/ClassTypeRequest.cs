using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class ClassTypeRequest
    {
        public int id { get; set; }
        [Required(ErrorMessage = "Tên loại lớp không được bỏ trống")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Trạng thái không được bỏ trống")]
        public bool? Status { get; set; }
        [Required(ErrorMessage = "Ghi chú không được bỏ trống")]
        public string? Note { get; set; }
    }
}