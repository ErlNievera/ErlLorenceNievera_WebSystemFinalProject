using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentGradeManagement.Application.DTOs.Auth;
using StudentGradeManagement.Application.Interfaces;
using StudentGradeManagement.Domain.Constants;
using StudentGradeManagement.Infrastructure.Identity;
using StudentGradeManagement.Infrastructure.Persistence;

namespace StudentGradeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AcademicDbContext _academicContext;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        AcademicDbContext academicContext,
        IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _academicContext = academicContext;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new
            {
                message = "Email and password are required."
            });
        }

        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.IsActive)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            request.Password,
            lockoutOnFailure: true);

        if (!signInResult.Succeeded)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Count != 1)
        {
            return Unauthorized(new
            {
                message = "Account does not have a valid application role."
            });
        }

        var role = roles[0];

        if (role == ApplicationRoles.Student)
        {
            var studentExists = await _academicContext.Students
                .AnyAsync(student =>
                    student.IdentityUserId == user.Id &&
                    student.IsActive);

            if (!studentExists)
            {
                return Unauthorized(new
                {
                    message = "No active student record is linked to this account."
                });
            }
        }
        else if (role == ApplicationRoles.Faculty)
        {
            var facultyExists = await _academicContext.Faculty
                .AnyAsync(faculty =>
                    faculty.IdentityUserId == user.Id &&
                    faculty.IsActive);

            if (!facultyExists)
            {
                return Unauthorized(new
                {
                    message = "No active faculty record is linked to this account."
                });
            }
        }
        else
        {
            return Unauthorized(new
            {
                message = "Account does not have an authorized role."
            });
        }

        var tokenResponse = _jwtTokenService.GenerateToken(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            roles);

        return Ok(tokenResponse);
    }
}