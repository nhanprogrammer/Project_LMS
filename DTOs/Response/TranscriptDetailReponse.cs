namespace Project_LMS.DTOs.Response
{
    public class TranscriptDetailReponse
    {
        //Học kỳ
        public string semesterName { get; set; }
        //Tên sinh viên
        public string StudentName { get; set; }
        //Ngày sinh
        public string DateOfBirth { get; set; }
        //Điểm trung bình cả năm
        public double totalYearScore { get; set; }
        //Trạng thái
        public string status { get; set; }
        //Ngày cập nhật
        public string updateAt { get; set; }
        public List<TranscriptDetailScoreReponse> scoreReponses { get; set; }
    }
}
