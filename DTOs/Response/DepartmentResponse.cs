namespace Project_LMS.DTOs.Response
{
    public class DepartmentResponse
    {
        public int DepartmentID { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public int UserId { get; set; }
        public string? FullName { get; set; }
    }
}