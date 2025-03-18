using System.Text.Json.Serialization;
using Project_LMS.Helpers;

namespace Project_LMS.DTOs.Request
{
    public class UpdateTestExamRequest
    {
        public int? SubjectId { get; set; }
        public string? Topic { get; set; }
        public int? SemestersId { get; set; }
        public int? DurationInMinutes { get; set; }

        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly? ExamDate { get; set; }

        public List<int>? ClassIds { get; set; }
        public string? ClassOption { get; set; }
        public int? SelectedClassTypeId { get; set; }
        public bool? ApplyExaminerForAllClasses { get; set; }
        public List<int>? ExaminerIds { get; set; }
        public List<ExaminerForClassRequestUpdate>? ExaminersForClass { get; set; }
        public string? Description { get; set; }
        public int? TestExamTypeId { get; set; }
        public bool? IsExam { get; set; }
        public string? Form { get; set; }
        public int? DepartmentId { get; set; }
    }

    public class ExaminerForClassRequestUpdate
    {
        public int ClassId { get; set; }
        public List<int> ExaminerIds { get; set; }
    }
}