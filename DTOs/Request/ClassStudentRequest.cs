using System.Text.Json.Serialization;

namespace Project_LMS.DTOs.Request
{
    public class ClassStudentRequest
    {
        public int? ClassId { get; set; }
        public int? UserId { get; set; }
        [JsonIgnore]
        public int? UserCreate { get; set; }
        [JsonIgnore]
        public int? UserUpdate { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
