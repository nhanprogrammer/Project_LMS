namespace Project_LMS.DTOs.Request
{
    public class PaginationRequest
    {
        public int PageNumber { get; set; } = 1;  // Trang mặc định là 1
        public int PageSize { get; set; } = 10;   // Số lượng phần tử trên mỗi trang mặc định là 10
    }
}
