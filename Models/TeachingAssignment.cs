﻿using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class TeachingAssignment
    {
        public TeachingAssignment()
        {
            QuestionsAnswers = new HashSet<QuestionsAnswer>();
            Subjects = new HashSet<Subject>();
            Topics = new HashSet<Topic>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public int ClassId { get; set; }
        public int SubjectId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual Class Class { get; set; } = null!;
        public virtual Subject Subject { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual ICollection<QuestionsAnswer> QuestionsAnswers { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }
    }
}
