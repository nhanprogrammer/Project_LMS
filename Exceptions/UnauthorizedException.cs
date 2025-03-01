namespace Project_LMS.Exceptions;

public class UnauthorizedException : CustomException
{
    public UnauthorizedException() : base("Unauthorized access.") { }
}
