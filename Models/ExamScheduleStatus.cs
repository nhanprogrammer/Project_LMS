using Project_LMS.Models;

public class ExamScheduleStatus
{
    public int Id { get; set; }
    public string Names { get; set; }


    public ICollection<TestExam> TestExams { get; set; }
}