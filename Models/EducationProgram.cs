namespace Project_LMS.Models
{
    public class EducationProgram
    {
        public int Id { get; set; }
        public int EducationId { get; set; }
        public int TrainingProgramId { get; set; }

        public virtual EducationInformation Education { get; set; }
        public virtual TrainingProgram TrainingProgram { get; set; }
    }
}