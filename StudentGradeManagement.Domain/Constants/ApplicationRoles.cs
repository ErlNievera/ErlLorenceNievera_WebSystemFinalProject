
namespace StudentGradeManagement.Domain.Constants;

public static class ApplicationRoles
{
    public const string Student = "Student";
    public const string Faculty = "Faculty";

    public static readonly IReadOnlyList<string> All =
        new[] { Student, Faculty };
}
