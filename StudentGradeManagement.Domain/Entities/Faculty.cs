
namespace StudentGradeManagement.Domain.Entities;

public class Faculty
{
    public int FacultyId { get; set; }

    public string FacultyNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string? IdentityUserId { get; set; }

    public ICollection<StudentGrade> Grades { get; set; } = new List<StudentGrade>();
}
