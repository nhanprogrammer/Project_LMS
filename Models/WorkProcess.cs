using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class WorkProcess
    {
        public WorkProcess()
        {
            WorkProcessUnits = new HashSet<WorkProcessUnit>();

        }
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? OrganizationUnit { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public bool? IsDeleted { get; set; }
        public virtual ICollection<WorkProcessUnit> WorkProcessUnits { get; set; } = new HashSet<WorkProcessUnit>();
        public virtual Position Position { get; set; }
        public virtual Department Department { get; set; }
        public virtual User User { get; set; }
    }
}
