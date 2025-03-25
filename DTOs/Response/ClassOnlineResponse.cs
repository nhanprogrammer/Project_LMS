namespace Project_LMS.DTOs.Response
{
    public class ClassOnlineResponse
    {
        public int Id { get; set; }
        public string TeacherName { get; set; }
        public string ClassCode { get; set; }
        public string ClassTitle { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? ClassDescription { get; set; }
        public int? MaxStudents { get; set; }
        public int? CurrentStudents { get; set; }
        public bool? ClassStatus { get; set; }
    }
}