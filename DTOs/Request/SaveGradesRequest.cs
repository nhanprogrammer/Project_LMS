namespace Project_LMS.DTOs.Request
{
    public class SaveGradesRequest
    {
        public int TestId { get; set; }
        public int? ClassId { get; set; }
        public List<StudentGradeRequestDto> Grades { get; set; }
    }

    public class StudentGradeRequestDto
    {
        public int StudentId { get; set; }
        public double? Score { get; set; }
        public string Comment { get; set; }
    }
}