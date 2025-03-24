namespace Project_LMS.DTOs.Response;

public class ChatResponse
{
    public int Id { get; set; }
    public string? Img { get; set; }
    public string? Name { get; set; }
    public string? UserRole { get; set; }
    public string? Content { get; set; }

    public ChatResponse(int id, string img, string name, string userRole, string content)
        {
            Id = id;
            Img = img;
            Name = name;
            UserRole = userRole;
            Content = content;
        }
}
