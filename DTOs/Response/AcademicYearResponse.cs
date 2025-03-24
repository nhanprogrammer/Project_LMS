using System.Text.Json.Serialization;
using Project_LMS.Helpers;

namespace Project_LMS.DTOs.Response;

public class AcademicYearResponse
{
    public int Id { get; set; }
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly StartDate { get; set; }
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateOnly EndDate { get; set; }
    public string Name => $"{StartDate.Year}-{EndDate.Year}";
    public bool? IsInherit { get; set; }
    public int? AcademicParent { get; set; }
    public List<SemesterResponse> Semesters { get; set; }
}
