
using System.ComponentModel.DataAnnotations;

namespace StudentGradeManagement.Application.DTOs.Grades;

public class UpdateGradeRequest
{
    [Range(1, int.MaxValue)]
    public int StudentId { get; set; }

    [Required, StringLength(20)]
    public string SubjectCode { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string SubjectName { get; set; } = string.Empty;

    [Range(0, 100)]
    public int Grade { get; set; }

    [Required, StringLength(30)]
    public string Semester { get; set; } = string.Empty;

    [Required, StringLength(20)]
    public string SchoolYear { get; set; } = string.Empty;
}
