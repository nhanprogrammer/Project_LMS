namespace Project_LMS.Models
{
    public class Province
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NameEn { get; set; }
        public string? FullName { get; set; }
        public string? FullNameEn { get; set; }
        public string? CodeName { get; set; }
        public int? AdministrativeUnitId { get; set; }
        public int? AdministrativeRegionId { get; set; }

        public virtual ICollection<District> Districts { get; set; } = new HashSet<District>();
    }
}