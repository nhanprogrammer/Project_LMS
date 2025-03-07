﻿using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Subject
    {
        public Subject()
        {
            SubjectsGroups = new HashSet<SubjectsGroup>();
            TeacherClassSubjects = new HashSet<TeacherClassSubject>();
            TeachingAssignments = new HashSet<TeachingAssignment>();
        }

        public int Id { get; set; }
        public int SubjectTypeId { get; set; }
        public int TeachingAssignmentId { get; set; }
        public int SubjectGroupId { get; set; }
        public bool? IsStatus { get; set; }
        public string? SubjectCode { get; set; }
        public string? SubjectName { get; set; }
        public string? Description { get; set; }
        public int? Semester1PeriodCount { get; set; }
        public int? Semester2PeriodCount { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual SubjectsGroup SubjectGroup { get; set; } = null!;
        public virtual SubjectType SubjectType { get; set; } = null!;
        public virtual TeachingAssignment TeachingAssignment { get; set; } = null!;
        public virtual ICollection<SubjectsGroup> SubjectsGroups { get; set; }
        public virtual ICollection<TeacherClassSubject> TeacherClassSubjects { get; set; }
        public virtual ICollection<TeachingAssignment> TeachingAssignments { get; set; }
    }
}
