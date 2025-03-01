namespace Project_LMS.DTOs.Request;

public class NationalityRequest
{
    public int? Id { get; set; }
    public string ShortName { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string PhoneCode { get; set; } = null!;
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public bool? IsDelete { get; set; }
    public int? UserCreate { get; set; }
    public int? UserUpdate { get; set; }
}