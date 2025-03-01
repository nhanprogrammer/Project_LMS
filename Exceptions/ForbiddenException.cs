namespace Project_LMS.Exceptions;

public class ForbiddenException : CustomException
{
    public ForbiddenException() : base("Forbidden access.") { }
}