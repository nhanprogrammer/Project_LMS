using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class WorkProcessRequest
    {
        [Required(ErrorMessage = "UserId không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId không hợp lệ")]
        public int UserId { get; set; }
        public int AcademicYearId { get; set; }
        public int ClassId { get; set; }
        public string? Search { get; set; }
    }
    public class WorkProcessesResponse
    {
        public int Id { get; set; }
        public string? OrganizationUnit { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
    public class WorkProcessResponse
    {
        public int Id { get; set; }
        public string? OrganizationUnit { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
    public class WorkUnitResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }

    }

    public class WorkProcessCreateRequest
    {
        //public int Id { get; set; }
        [Required(ErrorMessage = "UserId không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId không hợp lệ")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "OrganizationUnit không được bỏ trống")]
        public string? OrganizationUnit { get; set; }
        [Required(ErrorMessage = "DepartmentId không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "DepartmentId không hợp lệ")]
        public int DepartmentId { get; set; }
        [Required(ErrorMessage = "PositionId không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "PositionId không hợp lệ")]
        public int PositionId { get; set; }
        [Required(ErrorMessage = "StartDate bắt buộc")]
        public string? StartDate { get; set; }
        [Required(ErrorMessage = "EndDate bắt buộc")]
        public string? EndDate { get; set; }
        public List<int> WorkUnitIds { get; set; }

    }
    public class WorkProcessUpdateRequest
    {
        [Required(ErrorMessage = "Id bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Id không hợp lệ")]
        public int Id { get; set; }
        [Required(ErrorMessage = "UserId không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId không hợp lệ")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "OrganizationUnit không được bỏ trống")]
        public string? OrganizationUnit { get; set; }
        [Required(ErrorMessage = "DepartmentId không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "DepartmentId không hợp lệ")]
        public int DepartmentId { get; set; }
        [Required(ErrorMessage = "PositionId không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "PositionId không hợp lệ")]
        public int PositionId { get; set; }
        [Required(ErrorMessage = "StartDate không được bỏ trống")]
        public string? StartDate { get; set; }
        [Required(ErrorMessage = "EndDate không được bỏ trống")]
        public string? EndDate { get; set; }
        public List<int> WorkUnitIds { get; set; }

    }

    public class WorkProcessDeleteRequest
    {
        [Required(ErrorMessage = "Id không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "Id không được bỏ trống")]
        public int Id { get; set; }

    }
    public class WorkUnitRequest
    {
        public string? Ids { get; set; }
    }
}
