using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class QuestionsAnswer
    {
        public QuestionsAnswer()
        {
            Favourites = new HashSet<Favourite>();
            InverseQuestionsAnswerNavigation = new HashSet<QuestionsAnswer>();
            QuestionsAnswerTopicViews = new HashSet<QuestionsAnswerTopicView>();
        }

        public int Id { get; set; }
        public int? TeachingAssignmentId { get; set; }
        public int? UserId { get; set; }
        public int? QuestionsAnswerId { get; set; }
        public string? Message { get; set; }
        public string? FileName { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? CreateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }

        public virtual QuestionsAnswer? QuestionsAnswerNavigation { get; set; }
        public virtual TeachingAssignment? TeachingAssignment { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Favourite> Favourites { get; set; }
        public virtual ICollection<QuestionsAnswer> InverseQuestionsAnswerNavigation { get; set; }
        public virtual ICollection<QuestionsAnswerTopicView> QuestionsAnswerTopicViews { get; set; }
    }
}
