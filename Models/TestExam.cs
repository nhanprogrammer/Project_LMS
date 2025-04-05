﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_LMS.Models
{
    public partial class TestExam
    {
        public TestExam()
        {
            Assignments = new HashSet<Assignment>();
            ClassTestExams = new HashSet<ClassTestExam>();
            Examiners = new HashSet<Examiner>();
            Questions = new HashSet<Question>();
        }

        public int Id { get; set; }
        public int? TestExamTypeId { get; set; }
        public int? SemestersId { get; set; }
        public int? ClassId { get; set; }
        public int? UserId { get; set; }
        public string? Topic { get; set; }
        public bool? IsExam { get; set; }
        public string? Form { get; set; }
        public TimeOnly? Duration { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string? Description { get; set; }
        public string? Attachment { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDelete { get; set; }
        public int? SubjectId { get; set; }

        public bool IsAttachmentRequired { get; set; }
        [ForeignKey("ScheduleStatusId")] public ExamScheduleStatus ExamScheduleStatus { get; set; }
        [Column("schedule_status_id")] public int? ScheduleStatusId { get; set; }
        public int? DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public virtual Class? Class { get; set; }
        public virtual Semester? Semesters { get; set; }
        public virtual TestExamType? TestExamType { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Assignment> Assignments { get; set; }
        public virtual ICollection<ClassTestExam> ClassTestExams { get; set; }
        public virtual ICollection<Examiner> Examiners { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual Subject Subject { get; set; }
    }
}