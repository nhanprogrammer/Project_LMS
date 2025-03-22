using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class QuestionAnswerTopicView
    {
        public int Id { get; set; }
        public int? QuestionsAnswerId { get; set; }
        public int? UserId { get; set; }
        public int? TopicId { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        // -- Navigation Properties:
        public virtual Topic? Topic { get; set; } // trỏ đến bảng Topics
        public virtual User? User { get; set; } // trỏ đến bảng Users   
        public virtual QuestionAnswer? QuestionAnswer { get; set; }
    }
}