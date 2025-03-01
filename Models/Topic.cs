using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class Topic
    {
        public Topic()
        {
            Favourites = new HashSet<Favourite>();
            InverseTopicNavigation = new HashSet<Topic>();
            QuestionsAnswerTopicViews = new HashSet<QuestionsAnswerTopicView>();
        }

        public int Id { get; set; }
        public int? TeachingAssignmentId { get; set; }
        public int? UserId { get; set; }
        public int? TopicId { get; set; }
        public DateTime? UpdateAt { get; set; }
        public DateTime? CreateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
        public bool? IsDelete { get; set; }
        public string? Title { get; set; }
        public string? FileName { get; set; }
        public string? Description { get; set; }
        public DateTime? CloseAt { get; set; }

        public virtual TeachingAssignment? TeachingAssignment { get; set; }
        public virtual Topic? TopicNavigation { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Favourite> Favourites { get; set; }
        public virtual ICollection<Topic> InverseTopicNavigation { get; set; }
        public virtual ICollection<QuestionsAnswerTopicView> QuestionsAnswerTopicViews { get; set; }
    }
}
