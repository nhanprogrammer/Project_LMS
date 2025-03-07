namespace Project_LMS.Models
{
    public class ClassStudent
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }

        public User User { get; set; }= null!;
        public Class Class { get; set; }= null!;
    }
}