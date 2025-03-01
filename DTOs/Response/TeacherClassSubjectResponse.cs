namespace Project_LMS.DTOs.Response
{
    public class TeacherClassSubjectResponse
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int ClassId { get; set; }
        public int SubjectsId { get; set; }
        public bool? IsPrimary { get; set; }
    }
}
