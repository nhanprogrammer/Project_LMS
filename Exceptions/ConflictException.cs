namespace Project_LMS.Exceptions;

public class ConflictException : CustomException
{
    public ConflictException(string message) : base(message) { }
}