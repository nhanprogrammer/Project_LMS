// DTOs/Request/SubjectGroupRequest.cs
namespace Project_LMS.DTOs.Request
{
    public class SubjectGroupRequest
    {
        public int SubjectId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsStatus { get; set; }
    }
}

