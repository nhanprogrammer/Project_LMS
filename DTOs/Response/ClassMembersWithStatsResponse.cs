namespace Project_LMS.DTOs.Response;

public class ClassMembersWithStatsResponse
{
    public IEnumerable<ClassMemberResponse> Members { get; set; }
    public int TotalViews { get; set; }
}

