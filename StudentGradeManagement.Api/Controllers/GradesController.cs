
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGradeManagement.Application.DTOs.Grades;
using StudentGradeManagement.Application.Services;
using StudentGradeManagement.Domain.Constants;
using StudentGradeManagement.Domain.Entities;
using StudentGradeManagement.Infrastructure.Persistence;

namespace StudentGradeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GradesController : ControllerBase
{
    private readonly AcademicDbContext _context;

    public GradesController(AcademicDbContext context)
    {
        _context = context;
    }

    private string? CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.FindFirstValue("sub");

    // ==========================================
    // STUDENT: VIEW OWN PROFILE
    // GET: api/Grades/my/profile
    // ==========================================

    [HttpGet("my/profile")]
    [Authorize(Roles = ApplicationRoles.Student)]
    public async Task<IActionResult> GetMyProfile()
    {
        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s =>
                s.IdentityUserId == CurrentUserId &&
                s.IsActive);

        if (student == null)
        {
            return NotFound(new
            {
                message = "Student record not found."
            });
        }

        return Ok(new
        {
            studentName = $"{student.FirstName} {student.LastName}",
            studentNumber = student.StudentNumber
        });
    }

    // ==========================================
    // STUDENT: VIEW OWN GRADES
    // GET: api/Grades/my
    // ==========================================

    [HttpGet("my")]
    [Authorize(Roles = ApplicationRoles.Student)]
    public async Task<IActionResult> GetMyGrades()
    {
        var student = await _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s =>
                s.IdentityUserId == CurrentUserId &&
                s.IsActive);

        if (student == null)
        {
            return NotFound(new
            {
                message = "Student record not found."
            });
        }

        var grades = await _context.StudentGrades
            .AsNoTracking()
            .Include(g => g.Student)
            .Include(g => g.Faculty)
            .Where(g => g.StudentId == student.StudentId)
            .OrderBy(g => g.SubjectCode)
            .Select(g => ToResponse(g))
            .ToListAsync();

        return Ok(grades);
    }

    // ==========================================
    // FACULTY: VIEW ALL GRADES
    // GET: api/Grades
    // ==========================================

    [HttpGet]
    [Authorize(Roles = ApplicationRoles.Faculty)]
    public async Task<IActionResult> GetAllGrades()
    {
        var grades = await _context.StudentGrades
            .AsNoTracking()
            .Include(g => g.Student)
            .Include(g => g.Faculty)
            .OrderBy(g => g.Student.StudentNumber)
            .Select(g => ToResponse(g))
            .ToListAsync();

        return Ok(grades);
    }

    // ==========================================
    // FACULTY: VIEW ONE GRADE
    // GET: api/Grades/{id}
    // ==========================================

    [HttpGet("{id:int}")]
    [Authorize(Roles = ApplicationRoles.Faculty)]
    public async Task<IActionResult> GetGrade(int id)
    {
        var grade = await _context.StudentGrades
            .AsNoTracking()
            .Include(g => g.Student)
            .Include(g => g.Faculty)
            .FirstOrDefaultAsync(g => g.GradeId == id);

        if (grade == null)
        {
            return NotFound(new
            {
                message = "Grade record not found."
            });
        }

        return Ok(ToResponse(grade));
    }

    // ==========================================
    // FACULTY: CREATE GRADE
    // POST: api/Grades
    // ==========================================

    [HttpPost]
    [Authorize(Roles = ApplicationRoles.Faculty)]
    public async Task<IActionResult> CreateGrade(
        CreateGradeRequest request)
    {
        var faculty = await GetCurrentFacultyAsync();

        if (faculty == null)
            return Forbid();

        var student = await _context.Students
            .FirstOrDefaultAsync(s =>
                s.StudentId == request.StudentId &&
                s.IsActive);

        if (student == null)
        {
            return NotFound(new
            {
                message = "Active student not found."
            });
        }

        var duplicate = await _context.StudentGrades.AnyAsync(g =>
            g.StudentId == request.StudentId &&
            g.SubjectCode == request.SubjectCode &&
            g.Semester == request.Semester &&
            g.SchoolYear == request.SchoolYear);

        if (duplicate)
        {
            return Conflict(new
            {
                message = "This student already has a grade for this subject, semester, and school year."
            });
        }

        var grade = new StudentGrade
        {
            StudentId = student.StudentId,
            SubjectCode = request.SubjectCode.Trim(),
            SubjectName = request.SubjectName.Trim(),
            FacultyId = faculty.FacultyId,
            Grade = request.Grade,
            Remarks = GradeRemarksCalculator.Calculate(request.Grade),
            Semester = request.Semester.Trim(),
            SchoolYear = request.SchoolYear.Trim(),
            DatePosted = DateTime.UtcNow
        };

        _context.StudentGrades.Add(grade);

        await _context.SaveChangesAsync();

        await _context.Entry(grade)
            .Reference(g => g.Student)
            .LoadAsync();

        await _context.Entry(grade)
            .Reference(g => g.Faculty)
            .LoadAsync();

        return CreatedAtAction(
            nameof(GetGrade),
            new { id = grade.GradeId },
            ToResponse(grade));
    }

    // ==========================================
    // FACULTY: UPDATE GRADE
    // PUT: api/Grades/{id}
    // ==========================================

    [HttpPut("{id:int}")]
    [Authorize(Roles = ApplicationRoles.Faculty)]
    public async Task<IActionResult> UpdateGrade(
        int id,
        UpdateGradeRequest request)
    {
        var faculty = await GetCurrentFacultyAsync();

        if (faculty == null)
            return Forbid();

        var grade = await _context.StudentGrades
            .FirstOrDefaultAsync(g => g.GradeId == id);

        if (grade == null)
        {
            return NotFound(new
            {
                message = "Grade record not found."
            });
        }

        var studentExists = await _context.Students.AnyAsync(s =>
            s.StudentId == request.StudentId &&
            s.IsActive);

        if (!studentExists)
        {
            return NotFound(new
            {
                message = "Active student not found."
            });
        }

        var duplicate = await _context.StudentGrades.AnyAsync(g =>
            g.GradeId != id &&
            g.StudentId == request.StudentId &&
            g.SubjectCode == request.SubjectCode &&
            g.Semester == request.Semester &&
            g.SchoolYear == request.SchoolYear);

        if (duplicate)
        {
            return Conflict(new
            {
                message = "Another grade already exists for this subject, semester, and school year."
            });
        }

        grade.StudentId = request.StudentId;
        grade.SubjectCode = request.SubjectCode.Trim();
        grade.SubjectName = request.SubjectName.Trim();
        grade.FacultyId = faculty.FacultyId;
        grade.Grade = request.Grade;
        grade.Remarks = GradeRemarksCalculator.Calculate(request.Grade);
        grade.Semester = request.Semester.Trim();
        grade.SchoolYear = request.SchoolYear.Trim();
        grade.DateModified = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await _context.Entry(grade)
            .Reference(g => g.Student)
            .LoadAsync();

        await _context.Entry(grade)
            .Reference(g => g.Faculty)
            .LoadAsync();

        return Ok(ToResponse(grade));
    }

    // ==========================================
    // FACULTY: DELETE GRADE
    // DELETE: api/Grades/{id}
    // ==========================================

    [HttpDelete("{id:int}")]
    [Authorize(Roles = ApplicationRoles.Faculty)]
    public async Task<IActionResult> DeleteGrade(int id)
    {
        var grade = await _context.StudentGrades
            .FirstOrDefaultAsync(g => g.GradeId == id);

        if (grade == null)
        {
            return NotFound(new
            {
                message = "Grade record not found."
            });
        }

        _context.StudentGrades.Remove(grade);

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ==========================================
    // FACULTY: GET STUDENTS
    // GET: api/Grades/students
    // ==========================================

    [HttpGet("students")]
    [Authorize(Roles = ApplicationRoles.Faculty)]
    public async Task<IActionResult> GetStudents()
    {
        var students = await _context.Students
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.StudentNumber)
            .Select(s => new
            {
                s.StudentId,
                s.StudentNumber,
                StudentName = s.FirstName + " " + s.LastName
            })
            .ToListAsync();

        return Ok(students);
    }

    // ==========================================
    // HELPER: GET CURRENT FACULTY
    // ==========================================

    private async Task<Faculty?> GetCurrentFacultyAsync()
    {
        return await _context.Faculty
            .FirstOrDefaultAsync(f =>
                f.IdentityUserId == CurrentUserId &&
                f.IsActive);
    }

    // ==========================================
    // HELPER: CONVERT GRADE TO RESPONSE
    // ==========================================

    private static GradeResponse ToResponse(StudentGrade grade)
    {
        return new GradeResponse
        {
            GradeId = grade.GradeId,
            StudentNumber = grade.Student.StudentNumber,
            StudentName = $"{grade.Student.FirstName} {grade.Student.LastName}",
            StudentId = grade.StudentId,
            SubjectCode = grade.SubjectCode,
            SubjectName = grade.SubjectName,
            FacultyName = $"{grade.Faculty.FirstName} {grade.Faculty.LastName}",
            Grade = grade.Grade,
            Remarks = grade.Remarks,
            Semester = grade.Semester,
            SchoolYear = grade.SchoolYear,
            DatePosted = grade.DatePosted,
            DateModified = grade.DateModified
        };
    }
}
