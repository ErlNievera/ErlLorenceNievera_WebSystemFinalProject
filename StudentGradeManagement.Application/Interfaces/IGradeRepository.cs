
using StudentGradeManagement.Domain.Entities;

namespace StudentGradeManagement.Application.Interfaces;

public interface IGradeRepository
{
    Task<List<StudentGrade>> GetAllAsync();
    Task<List<StudentGrade>> GetByStudentAsync(int studentId);
    Task<StudentGrade?> GetByIdAsync(int gradeId);
    Task<bool> StudentExistsAsync(int studentId);
    Task<bool> GradeExistsAsync(
        int studentId,
        string subjectCode,
        string semester,
        string schoolYear,
        int? excludeGradeId = null);
    Task<List<Student>> GetActiveStudentsAsync();
    Task<Student?> GetActiveStudentByUserIdAsync(string userId);
    Task<Faculty?> GetActiveFacultyByUserIdAsync(string userId);
    Task AddAsync(StudentGrade grade);
    Task SaveChangesAsync();
    Task DeleteAsync(StudentGrade grade);
}
