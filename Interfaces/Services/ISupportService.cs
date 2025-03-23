namespace Project_LMS.Interfaces.Services;

public interface ISupportService
{
    public Task SendEmailAsync( string subject, string body, string senderEmail);

}