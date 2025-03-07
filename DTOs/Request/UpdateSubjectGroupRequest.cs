namespace Project_LMS.DTOs.Request
{
    public class UpdateSubjectGroupRequest
    {
        public string Name { get; set; }
        public int UserId { get; set; }
        public List<int> SubjectIds { get; set; }
        
    }
}