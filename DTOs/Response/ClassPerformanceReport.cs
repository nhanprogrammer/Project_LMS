namespace Project_LMS.DTOs.Response
{
    public class ClassPerformanceReport
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int ExcellentCount { get; set; } // Giỏi
        public int GoodCount { get; set; }      // Khá
        public int AverageCount { get; set; }  // Trung Bình
        public int WeakCount { get; set; }     // Yếu
    }
}