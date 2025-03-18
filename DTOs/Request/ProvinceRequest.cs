namespace Project_LMS.DTOs.Request;

public class ProvinceRequest
{
    public int? Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string NameEn { get; set; } = null!;
    public string CodeName { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string FullNameEnd { get; set; } = null!;
    public bool? IsDelete { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UserCreate { get; set; }
    public int? UserUpdate { get; set; }
}