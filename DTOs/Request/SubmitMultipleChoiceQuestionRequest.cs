namespace Project_LMS.DTOs.Request;

public class SubmitMultipleChoiceQuestionRequest
{
    public int TestExamId { get; set; }

    public List<int> AnswerIds { get; set; }
}