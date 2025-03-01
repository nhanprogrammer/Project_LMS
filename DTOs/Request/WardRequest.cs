namespace Project_LMS.DTOs.Request
{
    public class WardRequest
    {
        public string Code { get; set; } = null!;
        public int DistrictId { get; set; }
        public string Name { get; set; } = null!;
        public string NameEn { get; set; } = null!;
        public string CodeName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public int? UserCreate { get; set; }
        public int? UserUpdate { get; set; }
    }
}
