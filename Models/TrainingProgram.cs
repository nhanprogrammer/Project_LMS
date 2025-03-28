namespace Project_LMS.Models
{
    public class TrainingProgram
    { public TrainingProgram()
        {
            EducationPrograms = new HashSet<EducationProgram>();
          
        }
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }

        public virtual ICollection<EducationProgram> EducationPrograms { get; set; } = new HashSet<EducationProgram>();
    }
}