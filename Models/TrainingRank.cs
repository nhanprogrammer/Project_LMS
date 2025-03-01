using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class TrainingRank
    {
        public TrainingRank()
        {
            Students = new HashSet<Student>();
            UserTrainingRanks = new HashSet<UserTrainingRank>();
        }

        public int Id { get; set; }
        public string? EducationLevel { get; set; }
        public string? FormTraining { get; set; }
        public bool? IsYear { get; set; }
        public bool? IsModule { get; set; }
        public int? Year { get; set; }
        public int? SemesterYear { get; set; }
        public int? RequiredModule { get; set; }
        public int? ElectiveModule { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<UserTrainingRank> UserTrainingRanks { get; set; }
    }
}
