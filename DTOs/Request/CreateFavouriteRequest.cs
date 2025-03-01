namespace Project_LMS.DTOs.Request;

public class CreateFavouriteRequest
{
    public int? QuestionsAnswerId { get; set; }
    public int? UserId { get; set; }
    public int? TopicId { get; set; }
}