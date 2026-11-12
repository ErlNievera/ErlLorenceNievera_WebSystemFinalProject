
using StudentGradeManagement.Application.DTOs.Grades;
using StudentGradeManagement.Application.Interfaces;
using StudentGradeManagement.Domain.Entities;

namespace StudentGradeManagement.Application.Services;

public class GradeService : IGradeService
{
    private readonly IGradeRepository _repository;

    public GradeService(IGradeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GradeResponse>> GetAllGradesAsync()
    {
        var grades = await _repository.GetAllAsync();

        return grades.Select(MapToResponse).ToList();
    }

    public async Task<List<GradeResponse>?> GetMyGradesAsync(string userId)
    {
        var student = await _repository.GetActiveStudentByUserIdAsync(userId);

        if (student is null)
        {
            return null;
        }

        var grades = await _repository.GetByStudentAsync(student.StudentId);

        return grades.Select(MapToResponse).ToList();
    }

    public async Task<GradeResponse?> GetGradeAsync(int id)
    {
        var grade = await _repository.GetByIdAsync(id);

        return grade is null ? null : MapToResponse(grade);
    }

    public async Task<GradeResponse> CreateGradeAsync(
        CreateGradeRequest request,
        string facultyUserId)
    {
        var faculty = await _repository.GetActiveFacultyByUserIdAsync(facultyUserId);

        if (faculty is null)
        {
            throw new UnauthorizedAccessException(
                "The faculty account is not linked to an active faculty record.");
        }

        var studentExists = await _repository.StudentExistsAsync(request.StudentId);

        if (!studentExists)
        {
            throw new KeyNotFoundException("The selected student does not exist or is inactive.");
        }

        var duplicate = await _repository.GradeExistsAsync(
            request.StudentId,
            request.SubjectCode,
            request.Semester,
            request.SchoolYear);

        if (duplicate)
        {
            throw new InvalidOperationException(
                "A grade for this student, subject, semester, and school year already exists.");
        }

        var grade = new StudentGrade
        {
            StudentId = request.StudentId,
            SubjectCode = request.SubjectCode.Trim(),
            SubjectName = request.SubjectName.Trim(),
            FacultyId = faculty.FacultyId,
            Grade = request.Grade,
            Remarks = GradeRemarksCalculator.Calculate(request.Grade),
            Semester = request.Semester.Trim(),
            SchoolYear = request.SchoolYear.Trim(),
            DatePosted = DateTime.UtcNow
        };

        await _repository.AddAsync(grade);
        await _repository.SaveChangesAsync();

        var savedGrade = await _repository.GetByIdAsync(grade.GradeId);

        return savedGrade is null
            ? throw new InvalidOperationException("The created grade could not be retrieved.")
            : MapToResponse(savedGrade);
    }

    public async Task<GradeResponse> UpdateGradeAsync(
        int id,
        UpdateGradeRequest request,
        string facultyUserId)
    {
        var faculty = await _repository.GetActiveFacultyByUserIdAsync(facultyUserId);

        if (faculty is null)
        {
            throw new UnauthorizedAccessException(
                "The faculty account is not linked to an active faculty record.");
        }

        var grade = await _repository.GetByIdAsync(id);

        if (grade is null)
        {
            throw new KeyNotFoundException("Grade record not found.");
        }

        var studentExists = await _repository.StudentExistsAsync(request.StudentId);

        if (!studentExists)
        {
            throw new KeyNotFoundException("The selected student does not exist or is inactive.");
        }

        var duplicate = await _repository.GradeExistsAsync(
            request.StudentId,
            request.SubjectCode,
            request.Semester,
            request.SchoolYear,
            id);

        if (duplicate)
        {
            throw new InvalidOperationException(
                "A grade for this student, subject, semester, and school year already exists.");
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

        await _repository.SaveChangesAsync();

        var updatedGrade = await _repository.GetByIdAsync(id);

        return updatedGrade is null
            ? throw new InvalidOperationException("The updated grade could not be retrieved.")
            : MapToResponse(updatedGrade);
    }

    public async Task<bool> DeleteGradeAsync(int id)
    {
        var grade = await _repository.GetByIdAsync(id);

        if (grade is null)
        {
            return false;
        }

        await _repository.DeleteAsync(grade);
        await _repository.SaveChangesAsync();

        return true;
    }

    public async Task<List<StudentOptionResponse>> GetStudentsAsync()
    {
        var students = await _repository.GetActiveStudentsAsync();

        return students.Select(student => new StudentOptionResponse
        {
            StudentId = student.StudentId,
            StudentNumber = student.StudentNumber,
            StudentName = $"{student.FirstName} {student.LastName}"
        }).ToList();
    }

    private static GradeResponse MapToResponse(StudentGrade grade)
    {
        return new GradeResponse
        {
            GradeId = grade.GradeId,
            StudentId = grade.StudentId,
            StudentNumber = grade.Student.StudentNumber,
            StudentName = $"{grade.Student.FirstName} {grade.Student.LastName}",
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
    public async Task<StudentProfileResponse?> GetMyProfileAsync(string userId)
    {
        var student = await _repository.GetActiveStudentByUserIdAsync(userId);

        if (student is null)
            return null;

        return new StudentProfileResponse
        {
            StudentNumber = student.StudentNumber,
            StudentName = $"{student.FirstName} {student.LastName}"
        };
    }
}
