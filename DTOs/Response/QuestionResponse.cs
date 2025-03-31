namespace Project_LMS.DTOs.Response
{
    public class QuestionResponse
    {
        public string QuestionNumber { get; set; }
        public int QuestionId { get; set; }        
        public string SubjectName { get; set; } 
        public string StartDate { get; set; }      
        public string Duration { get; set; }      
        public string Title { get; set; }     
        public string ClassName { get; set; }
        public string Question { get; set; }       
        public List<AnswerResponseByQuestionId> Answers { get; set; } 
    }

    public class AnswerResponseByQuestionId
    {
        public int AnswerId { get; set; } 
        public string AnswerText { get; set; }  
    }
}