using System.ComponentModel.DataAnnotations;

namespace Project_LMS.DTOs.Request
{
    public class EducationInformationRequest
    {
        [Required(ErrorMessage = "UserId không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId không hợp lệ")]
        public int UserId { get; set; }
        public int? AcademicYearId { get; set; }
        public int? ClassId { get; set; }
        public string? Search { get; set; }
    }
    public class EducationInformationsResponse
    {
        public int Id { get; set; }
        public string? TrainingInstitution { get; set; }
        public string? Major { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? TrainingForm { get; set; }
        public string? CertifiedDegree { get; set; }

    }
    public class EducationInformationResponse
    {
        public int Id { get; set; }
        public string? TrainingInstitution { get; set; }
        public string? Major { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? TrainingForm { get; set; }
        public string? CertifiedDegree { get; set; }
        public string? AttachedFile { get; set; }

    }
    public class TrainingProgramResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }

    }

      public class EducationInformationCreateRequest
    {
        //public int Id { get; set; }
        [Required(ErrorMessage = "UserId không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId không hợp lệ")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "TrainingInstitution không được bỏ trống")]
        public string? TrainingInstitution { get; set; }
        [Required(ErrorMessage = "Major không được bỏ trống")]
        public string? Major { get; set; }
        [Required(ErrorMessage = "StartDate không được bỏ trống")]
        public string? StartDate { get; set; }
        [Required(ErrorMessage = "EndDate không được bỏ trống")]
        public string? EndDate { get; set; }
        [Required(ErrorMessage = "TrainingForm không được bỏ trống")]
        public string? TrainingForm { get; set; }
        [Required(ErrorMessage = "CertifiedDegree không được bỏ trống")]
        public string? CertifiedDegree { get; set; }
        [Required(ErrorMessage = "AttachedFile không được bỏ trống")]
        public string? AttachedFile { get; set; }
        public List<int> TrainingProgramIds { get; set; }

    }
     public class EducationInformationUpdateRequest
    {
        [Required(ErrorMessage = "Id không được bỏ trống")]
        public int Id { get; set; }
        [Required(ErrorMessage = "UserId không được bỏ trống")]
        [Range(1, int.MaxValue, ErrorMessage = "UserId không hợp lệ")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "TrainingInstitution không được bỏ trống")]
        public string? TrainingInstitution { get; set; }
        [Required(ErrorMessage = "Major không được bỏ trống")]
        public string? Major { get; set; }
        [Required(ErrorMessage = "StartDate không được bỏ trống")]
        public string? StartDate { get; set; }
        [Required(ErrorMessage = "EndDate không được bỏ trống")]
        public string? EndDate { get; set; }
        [Required(ErrorMessage = "TrainingForm không được bỏ trống")]
        public string? TrainingForm { get; set; }
        [Required(ErrorMessage = "CertifiedDegree không được bỏ trống")]
        public string? CertifiedDegree { get; set; }
        [Required(ErrorMessage = "AttachedFile không được bỏ trống")]
        public string? AttachedFile { get; set; }
        public List<int> TrainingProgramIds { get; set; }

    }

    public class EducationInformationDeleteRequest
    {
        [Required(ErrorMessage = "Id không được bỏ trống")]
        public int Id { get; set; }

    }
      public class TrainingProgramRequest
    {
        public string? Ids { get; set; }
    }
}
