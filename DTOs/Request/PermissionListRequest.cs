namespace Project_LMS.DTOs.Request
{
    public class PermissionListRequest
    {
        public int PageNumber { get; set; } = 1; 
        public int PageSize { get; set; } = 10; 
        public string? Key { get; set; }
    }
}
   