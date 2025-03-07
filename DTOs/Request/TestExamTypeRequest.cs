namespace Project_LMS.DTOs.Request
{
    public class TestExamTypeRequest
    {
        public string PointTypeName { get; set; } = null!;
        public int? Coefficient { get; set; }
        public int? MinimunEntriesSem1 { get; set; }
        public int? MinimunEntriesSem2 { get; set; }
    }
}