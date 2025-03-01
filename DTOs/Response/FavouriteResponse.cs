namespace Project_LMS.DTOs.Response;

public class FavouriteResponse
{
    public int Id { get; set; }
    public int? QuestionsAnswerId { get; set; }
    public int? UserId { get; set; }
    public int? TopicId { get; set; }
}