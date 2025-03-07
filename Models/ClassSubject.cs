namespace Project_LMS.Models
{
    public class ClassSubject
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int ClassId { get; set; }

        public Subject Subject { get; set; }= null!;
        public Class Class { get; set; }= null!;
    }
}