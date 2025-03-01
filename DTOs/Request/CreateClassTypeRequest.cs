namespace Project_LMS.DTOs.Request;

    public class CreateClassTypeRequest
    {
        public string Name { get; set; }
        public DateTime? CreateAt { get; set; } = DateTime.UtcNow;
        public int UserCreate { get; set; }
    }