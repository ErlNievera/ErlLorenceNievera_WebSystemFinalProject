
using StudentGradeManagement.Application.DTOs.Grades;

namespace StudentGradeManagement.Application.Interfaces;

public interface IGradeService
{
    Task<List<GradeResponse>> GetAllGradesAsync();
    Task<List<GradeResponse>?> GetMyGradesAsync(string userId);
    Task<GradeResponse?> GetGradeAsync(int id);
    Task<GradeResponse> CreateGradeAsync(
        CreateGradeRequest request,
        string facultyUserId);
    Task<GradeResponse> UpdateGradeAsync(
        int id,
        UpdateGradeRequest request,
        string facultyUserId);
    Task<bool> DeleteGradeAsync(int id);
    Task<List<StudentOptionResponse>> GetStudentsAsync();
    Task<StudentProfileResponse?> GetMyProfileAsync(string userId);
}
