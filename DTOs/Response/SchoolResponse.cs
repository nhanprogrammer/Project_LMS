using System.Text.Json.Serialization;

namespace Project_LMS.DTOs.Response
{
  public class SchoolResponse
  {
    public int Id { get; set; }

    public string SchoolCode { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Principal { get; set; } = null!;

    public string PrincipalPhone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Website { get; set; } = null!;

    public string Province { get; set; } = null!;

    public string District { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public string? Fax { get; set; }

    public bool? IsJuniorHigh { get; set; }

    public bool? IsHighSchool { get; set; }

    public string EducationModel { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public DateTime EstablishmentDate { get; set; }

    public string? HeadOffice { get; set; }
    public string? Image { get; set; }

    private List<SchoolBranchResponse>? _branches;

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<SchoolBranchResponse>? Branches
    {
        set => _branches = value;                   
        get => _branches?.Any() == true ? _branches : null; 
    }
  }
}