
namespace StudentGradeManagement.Domain.Entities;

public class Student
{
    public int StudentId { get; set; }

    public string StudentNumber { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string? IdentityUserId { get; set; }

    public ICollection<StudentGrade> Grades { get; set; } = new List<StudentGrade>();
}
