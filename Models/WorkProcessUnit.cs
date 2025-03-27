namespace Project_LMS.Models
{
    public class WorkProcessUnit
    { 
        public int Id { get; set; }
        public int WorkProcessId { get; set; }
        public int WorkUnitId { get; set; }

        public virtual WorkProcess WorkProcess { get; set; }
        public virtual WorkUnit WorkUnit { get; set; }
    }
}