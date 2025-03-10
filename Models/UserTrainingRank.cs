using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class UserTrainingRank
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? TrainingRankId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual TrainingRank? TrainingRank { get; set; }
        public virtual User? User { get; set; }
    }
}
