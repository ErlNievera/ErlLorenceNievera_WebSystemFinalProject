using Microsoft.EntityFrameworkCore;
using StudentGradeManagement.Application.Interfaces;
using StudentGradeManagement.Domain.Entities;
using StudentGradeManagement.Infrastructure.Persistence;

namespace StudentGradeManagement.Infrastructure.Repositories;

public class GradeRepository : IGradeRepository
{
    private readonly AcademicDbContext _context;

    public GradeRepository(AcademicDbContext context)
    {
        _context = context;
    }

    public async Task<List<StudentGrade>> GetAllAsync()
    {
        return await _context.StudentGrades
            .AsNoTracking()
            .Include(g => g.Student)
            .Include(g => g.Faculty)
            .OrderBy(g => g.Student.StudentNumber)
            .ThenBy(g => g.SubjectCode)
            .ToListAsync();
    }

    public async Task<List<StudentGrade>> GetByStudentAsync(int studentId)
    {
        return await _context.StudentGrades
            .AsNoTracking()
            .Include(g => g.Student)
            .Include(g => g.Faculty)
            .Where(g => g.StudentId == studentId)
            .OrderBy(g => g.SubjectCode)
            .ToListAsync();
    }

    public async Task<StudentGrade?> GetByIdAsync(int gradeId)
    {
        return await _context.StudentGrades
            .Include(g => g.Student)
            .Include(g => g.Faculty)
            .FirstOrDefaultAsync(g => g.GradeId == gradeId);
    }

    public Task<bool> StudentExistsAsync(int studentId)
    {
        return _context.Students
            .AnyAsync(s => s.StudentId == studentId && s.IsActive);
    }

    public Task<bool> GradeExistsAsync(
        int studentId,
        string subjectCode,
        string semester,
        string schoolYear,
        int? excludeGradeId = null)
    {
        return _context.StudentGrades.AnyAsync(g =>
            g.StudentId == studentId &&
            g.SubjectCode == subjectCode &&
            g.Semester == semester &&
            g.SchoolYear == schoolYear &&
            (!excludeGradeId.HasValue || g.GradeId != excludeGradeId.Value));
    }

    public async Task<List<Student>> GetActiveStudentsAsync()
    {
        return await _context.Students
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.StudentNumber)
            .ToListAsync();
    }

    public Task<Student?> GetActiveStudentByUserIdAsync(string userId)
    {
        return _context.Students
            .FirstOrDefaultAsync(s =>
                s.IdentityUserId == userId && s.IsActive);
    }

    public Task<Faculty?> GetActiveFacultyByUserIdAsync(string userId)
    {
        return _context.Faculty
            .FirstOrDefaultAsync(f =>
                f.IdentityUserId == userId && f.IsActive);
    }

    public async Task AddAsync(StudentGrade grade)
    {
        await _context.StudentGrades.AddAsync(grade);
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public Task DeleteAsync(StudentGrade grade)
    {
        _context.StudentGrades.Remove(grade);
        return Task.CompletedTask;
    }
}
