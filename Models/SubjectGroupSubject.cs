


public partial class SubjectGroupSubject
{
    
    public int Id { get; set; }
    public int SubjectGroupId { get; set; }
    public int SubjectId { get; set; }

    public virtual SubjectsGroup SubjectsGroup { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
}