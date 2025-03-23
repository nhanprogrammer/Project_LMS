﻿using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Class
    {
        public Class()
        {
            ChatMessages = new HashSet<ChatMessage>();
            ClassStudentOnlines = new HashSet<ClassStudentOnline>();
            ClassStudents = new HashSet<ClassStudent>();
            ClassSubjects = new HashSet<ClassSubject>();
            ClassTestExams = new HashSet<ClassTestExam>();
            TeachingAssignments = new HashSet<TeachingAssignment>();
            TestExams = new HashSet<TestExam>();
        }

        public int Id { get; set; }
        public int? DepartmentId { get; set; }
        public int? ClassTypeId { get; set; }
        public int? AcademicYearId { get; set; }
        public int? UserId { get; set; }
        public string? Description { get; set; }

        public string? Name { get; set; }
        public int? StudentCount { get; set; }
        public string? ClassCode { get; set; }
        public string? PasswordClass { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StatusClass { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool? IsDelete { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual AcademicYear? AcademicYear { get; set; }
        public virtual ClassType? ClassType { get; set; }
        public virtual Department? Department { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
        public virtual ICollection<ClassStudentOnline> ClassStudentOnlines { get; set; }
        public virtual ICollection<ClassStudent> ClassStudents { get; set; }
        public virtual ICollection<ClassSubject> ClassSubjects { get; set; }
        public virtual ICollection<ClassTestExam> ClassTestExams { get; set; }
        public virtual ICollection<TeachingAssignment> TeachingAssignments { get; set; }
        public virtual ICollection<TestExam> TestExams { get; set; }
        public virtual ICollection<Examiner> Examiners { get; set; }
    }
}