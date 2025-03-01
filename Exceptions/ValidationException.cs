namespace Project_LMS.Exceptions;

public class ValidationException : CustomException
{
    public ValidationException(string message) : base(message) { }
}