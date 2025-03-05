using System;
using System.Collections.Generic;

namespace Project_LMS.Models
{
    public partial class QuestionsAnswerTopicView
    {
        public int Id { get; set; }
        public int? QuestionsAnswerId { get; set; }
        public int? UserId { get; set; }
        public int? TopicId { get; set; }
    }
}
