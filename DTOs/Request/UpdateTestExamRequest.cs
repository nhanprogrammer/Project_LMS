using System.Text.Json.Serialization;
using Project_LMS.Helpers;
using System.Collections.Generic;

namespace Project_LMS.DTOs.Request
{
    public class UpdateTestExamRequest
    {
        public int id { get; set; }
        public int? subjectId { get; set; }
        public string? topic { get; set; }
        public int? semestersId { get; set; }
        public int? durationInMinutes { get; set; }

        [JsonConverter(typeof(DateOnlyJsonConverter))]
        public DateOnly? examDate { get; set; }

        public List<int>? classIds { get; set; }
        public string? classOption { get; set; }
        public int? selectedClassTypeId { get; set; }
        public bool? applyExaminerForAllClasses { get; set; }
        public List<int>? examinerIds { get; set; }
        public List<ExaminerForClassRequestUpdate>? examinersForClass { get; set; }
        public string? description { get; set; }
        public int? testExamTypeId { get; set; }
        public bool? isExam { get; set; }
        public string? form { get; set; }
        public int? departmentId { get; set; }
    }

    public class ExaminerForClassRequestUpdate
    {
        public int classId { get; set; }
        public List<int> examinerIds { get; set; }
    }
}