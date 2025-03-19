namespace Project_LMS.DTOs.Response
{
    public class TranscriptReponse
    {
        //Tên môn học
        public string SubjectName { get; set; }
        //Tên lớp học
        public string ClassName { get; set; }
        //Mã lớp học
        public string ClassCode { get; set; }
        //Thời gian bắt đầu
        public string StartTime { get; set; }
        //Họ tên sinh viên
        public string StudentName { get; set; }
        //Ngày sinh
        public string DateOfBirth { get; set; }
        //Điểm chi tiết
        public List<TranscriptDetailReponse> transcriptDetails { get; set; }

    }
}
