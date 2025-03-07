using Project_LMS.Models;

public partial class SubjectsGroup
{
    public SubjectsGroup()
    {
        SubjectGroupSubjects = new HashSet<SubjectGroupSubject>();
    }

    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; } = null!;
    public bool? IsDelete { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UserCreate { get; set; }
    public int? UserUpdate { get; set; }

    public virtual ICollection<SubjectGroupSubject> SubjectGroupSubjects { get; set; }
    public virtual User User { get; set; } = null!;
}