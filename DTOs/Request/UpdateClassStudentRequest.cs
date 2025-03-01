namespace Project_LMS.DTOs.Request
{
    public class UpdateClassStudentRequest
    {
        public int Id { get; set; }
        public int? ClassId { get; set; }
        public int? StudentId { get; set; }
    }
}
