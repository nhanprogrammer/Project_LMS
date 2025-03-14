namespace Project_LMS.DTOs.Response;

public class ChatResponse
{
    public int Id { get; set; }
    public string? ClassCode { get; set; } //RoomId
    public string? Img { get; set; }
    public string? SenderName { get; set; }
    public string? UserRoleName { get; set; }
    public string? DayTime { get; set; }
    public string? Time { get; set; }
    public string? Content { get; set; }

    public ChatResponse(int id, string classcode, string img, string senderName, string userRoleName, DateTime dayTime, string content)
        {
            Id = id;
            ClassCode = classcode;
            Img = img;
            SenderName = senderName;
            UserRoleName = userRoleName;
            DayTime = dayTime.ToString("dd/MM/yy");
            Time = dayTime.ToString("HH:mm");
            Content = content;
        }
}
