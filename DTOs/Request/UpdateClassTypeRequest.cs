namespace Project_LMS.DTOs.Request
{
    public class UpdateClassTypeRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? UpdateAt { get; set; } = DateTime.UtcNow;
        public int UserUpdate { get; set; }
    }
}
