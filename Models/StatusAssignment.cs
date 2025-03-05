using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class StatusAssignment
    {
        public StatusAssignment()
        {
            Assignments = new HashSet<Assignment>();
        }

        public int Id { get; set; }
        public string? StatusName { get; set; }

        public virtual ICollection<Assignment> Assignments { get; set; }
    }
}
