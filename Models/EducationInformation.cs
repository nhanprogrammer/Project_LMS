using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class EducationInformation
    {
        public EducationInformation()
        {
            EducationPrograms = new HashSet<EducationProgram>();

        }
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? TrainingInstitution { get; set; }
        public string? Major { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? TrainingForm { get; set; }
        public string? CertifiedDegree { get; set; }
        public string? AttachedFile { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDeleted { get; set; }
        public virtual ICollection<EducationProgram> EducationPrograms { get; set; } = new HashSet<EducationProgram>();
        public virtual User User { get; set; }
    }
}
