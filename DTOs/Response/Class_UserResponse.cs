namespace Project_LMS.DTOs.Response
{
    public class Class_UserResponse
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
    }

    public class User_AcademicHoldsResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
    }


}
