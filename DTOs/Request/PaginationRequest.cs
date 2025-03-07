namespace Project_LMS.DTOs.Request
{
    public class PaginationRequest
    {
        public int PageNumber { get; set; } = 1;  // Trang mặc định là 1
        public int PageSize { get; set; }
    }
}
