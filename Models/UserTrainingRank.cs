using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class UserTrainingRank
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public int? TrainingRankId { get; set; }

        public virtual TrainingRank? TrainingRank { get; set; }
        public virtual User? User { get; set; }
    }
}
