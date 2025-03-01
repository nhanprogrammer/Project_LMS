namespace Project_LMS.DTOs.Request
{
    public class TeacherClassSubjectRequest
    {
        public int TeacherId { get; set; }
        public int ClassId { get; set; }
        public int SubjectsId { get; set; }
        public bool? IsPrimary { get; set; }
    }
}
