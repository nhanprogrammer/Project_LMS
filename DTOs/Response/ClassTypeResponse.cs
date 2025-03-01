namespace Project_LMS.DTOs.Response
{
    public class ClassTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}