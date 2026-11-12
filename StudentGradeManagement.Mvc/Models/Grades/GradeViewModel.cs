namespace StudentGradeManagement.Mvc.Models.Grades;

public class GradeViewModel
{
    public int GradeId { get; set; }
    public int StudentId { get; set; }
    public string StudentNumber { get; set; } = "";
    public string StudentName { get; set; } = "";
    public string SubjectCode { get; set; } = "";
    public string SubjectName { get; set; } = "";
    public string FacultyName { get; set; } = "";
    public int Grade { get; set; }
    public string Remarks { get; set; } = "";
    public string Semester { get; set; } = "";
    public string SchoolYear { get; set; } = "";
    public DateTime DatePosted { get; set; }
    public DateTime? DateModified { get; set; }
}