using System.Text.Json.Serialization;

public class SubjectGroupResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string FullName { get; set; } 
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<SubjectInfo> Subjects { get; set; }
}
public class SubjectInfo
{
    public int Id { get; set; }
    public string SubjectCode { get; set; }
    public string SubjectName { get; set; }
}

