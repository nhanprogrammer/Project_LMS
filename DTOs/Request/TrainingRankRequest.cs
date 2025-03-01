namespace Project_LMS.DTOs.Request
{
    public class TrainingRankRequest
    {
        public string? EducationLevel { get; set; }
        public string? FormTraining { get; set; }
        public bool? IsYear { get; set; }
        public bool? IsModule { get; set; }
        public int? Year { get; set; }
        public int? SemesterYear { get; set; }
        public int? RequiredModule { get; set; }
        public int? ElectiveModule { get; set; }
        public bool? IsActive { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
    }
}
