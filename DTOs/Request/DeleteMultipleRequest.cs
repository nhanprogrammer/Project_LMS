namespace Project_LMS.DTOs.Request
{
    public class DeleteMultipleRequest
    {
        public List<int> Ids { get; set; } = new List<int>();
    }
}