namespace Project_LMS.DTOs.Response
{
    public class TestExamTypeResponse
    {
        public int Id { get; set; }
        public string PointTypeName { get; set; } = null!;
        public int? Coefficient { get; set; }
        public int? MinimunEntriesSem1 { get; set; }
        public int? MinimunEntriesSem2 { get; set; }
    }
}