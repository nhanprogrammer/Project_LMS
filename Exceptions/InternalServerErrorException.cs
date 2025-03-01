namespace Project_LMS.Exceptions;

public class InternalServerErrorException : CustomException
{
    public InternalServerErrorException() : base("Server error...") { }
}