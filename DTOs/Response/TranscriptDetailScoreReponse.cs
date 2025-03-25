namespace Project_LMS.DTOs.Response
{
    public class TranscriptDetailScoreReponse
    {
        //Loại điểm
        public string pointTypeName { get; set; }
        //Hệ số
        public int coefficient { get; set; }
        //Điểm
        public int score { get; set; }
        //Điểm trung bình
        public double totalScore { get; set; }
    }
}
