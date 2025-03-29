namespace Project_LMS.Models
{
    public class District
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NameEn { get; set; }
        public string? FullName { get; set; }
        public string? FullNameEn { get; set; }
        public string? CodeName { get; set; }
        public int? ProvinceCode { get; set; }
        public int? AdministrativeUnitId { get; set; }

        public virtual Province? Province { get; set; }
        public virtual ICollection<Ward> Wards { get; set; } = new HashSet<Ward>();
    }
}