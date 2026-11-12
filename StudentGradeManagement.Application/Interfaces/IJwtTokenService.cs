
using StudentGradeManagement.Application.DTOs.Auth;

namespace StudentGradeManagement.Application.Interfaces;

public interface IJwtTokenService
{
    LoginResponse GenerateToken(
        string userId,
        string email,
        string firstName,
        string lastName,
        IList<string> roles);
}
