namespace Project_LMS.DTOs.Response
{
    public class ClassTypeResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? Status { get; set; }
        public string? Note { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
    }
}