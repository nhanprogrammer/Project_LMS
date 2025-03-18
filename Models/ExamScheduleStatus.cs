using System.ComponentModel.DataAnnotations.Schema;

namespace Project_LMS.Models;

public class ExamScheduleStatus
{
    [Column("id")]
    public int Id { get; set; }
    
    [Column("names")]
    public string Names { get; set; }
    
    public ICollection<TestExam> TestExams { get; set; }
}