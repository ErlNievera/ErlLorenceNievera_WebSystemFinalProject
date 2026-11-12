using System.ComponentModel.DataAnnotations;

namespace StudentGradeManagement.Mvc.Models.Grades;

public class GradeFormViewModel
{
    public int GradeId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Please select a student.")]
    public int StudentId { get; set; }

    [Required]
    [StringLength(20)]
    public string SubjectCode { get; set; } = "";

    [Required]
    [StringLength(100)]
    public string SubjectName { get; set; } = "";

    [Range(0, 100, ErrorMessage = "Grade must be between 0 and 100.")]
    public int Grade { get; set; }

    [Required]
    [StringLength(30)]
    public string Semester { get; set; } = "";

    [Required]
    [StringLength(20)]
    public string SchoolYear { get; set; } = "";

    public List<StudentOptionViewModel> Students { get; set; } = [];
}