namespace Project_LMS.DTOs.Request
{
    public class ExportSchoolExcelRequest
    {
        public int SchoolId { get; set; }
        public List<int>? SchoolBranchIds { get; set; }
    }
}