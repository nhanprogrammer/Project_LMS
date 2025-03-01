using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class User
    {
        public User()
        {
            AcademicHolds = new HashSet<AcademicHold>();
            ChatMessages = new HashSet<ChatMessage>();
            Favourites = new HashSet<Favourite>();
            Notifications = new HashSet<Notification>();
            NotificationsReceivers = new HashSet<NotificationsReceiver>();
            QuestionsAnswerTopicViews = new HashSet<QuestionsAnswerTopicView>();
            QuestionsAnswers = new HashSet<QuestionsAnswer>();
            Registrations = new HashSet<Registration>();
            Students = new HashSet<Student>();
            TeacherContacts = new HashSet<TeacherContact>();
            Teachers = new HashSet<Teacher>();
            Topics = new HashSet<Topic>();
            UserTrainingRanks = new HashSet<UserTrainingRank>();
        }

        public int Id { get; set; }
        public int? ConfigurationId { get; set; }
        public int? Role { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public bool? Active { get; set; }
        public bool? IsDelete { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }

        public virtual SystemSetting? Configuration { get; set; }
        public virtual ICollection<AcademicHold> AcademicHolds { get; set; }
        public virtual ICollection<ChatMessage> ChatMessages { get; set; }
        public virtual ICollection<Favourite> Favourites { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<NotificationsReceiver> NotificationsReceivers { get; set; }
        public virtual ICollection<QuestionsAnswerTopicView> QuestionsAnswerTopicViews { get; set; }
        public virtual ICollection<QuestionsAnswer> QuestionsAnswers { get; set; }
        public virtual ICollection<Registration> Registrations { get; set; }
        public virtual ICollection<Student> Students { get; set; }
        public virtual ICollection<TeacherContact> TeacherContacts { get; set; }
        public virtual ICollection<Teacher> Teachers { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }
        public virtual ICollection<UserTrainingRank> UserTrainingRanks { get; set; }
    }
}
