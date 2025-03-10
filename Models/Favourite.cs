using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Favourite
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

        public virtual QuestionAnswer? QuestionsAnswer { get; set; }
        public virtual Topic? Topic { get; set; }
        public virtual User? User { get; set; }
    }
}
