namespace Project_LMS.Exceptions;

public class NotFoundException : CustomException
{
    public NotFoundException(string message) : base(message) { }
}