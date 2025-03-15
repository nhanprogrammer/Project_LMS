namespace Project_LMS.Exceptions
{
    public class BadRequestException : CustomException
    {
        public List<ValidationError> Errors { get; }

        public BadRequestException(string message, List<ValidationError> errors) : base(message)
        {
            Errors = errors;
        }


        public static BadRequestException BadRequest(string message)
        {
            return new BadRequestException(message, new List<ValidationError>());
        }
    }
}