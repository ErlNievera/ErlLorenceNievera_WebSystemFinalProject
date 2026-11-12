
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentGradeManagement.Infrastructure.Persistence;

namespace StudentGradeManagement.Infrastructure.Identity;

public static class DemoAccountSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        AcademicDbContext academicContext,
        IConfiguration configuration)
    {
        var initialPassword =
            configuration["DemoAccounts:InitialPassword"]
            ?? throw new InvalidOperationException(
                "Demo account initial password is missing.");

        var resetExistingPasswords =
            configuration.GetValue<bool>(
                "DemoAccounts:ResetExistingPasswords");

        var students = await academicContext.Students
            .Where(student => student.IsActive)
            .ToListAsync();

        var facultyMembers = await academicContext.Faculty
            .Where(faculty => faculty.IsActive)
            .ToListAsync();

        foreach (var student in students)
        {
            var user = await GetOrCreateUserAsync(
                userManager,
                student.Email,
                student.FirstName,
                student.LastName,
                initialPassword,
                resetExistingPasswords);

            if (student.IdentityUserId != null &&
                student.IdentityUserId != user.Id)
            {
                throw new InvalidOperationException(
                    $"Student {student.StudentNumber} is linked to another account.");
            }

            var linkedToFaculty = await academicContext.Faculty
                .AnyAsync(faculty => faculty.IdentityUserId == user.Id);

            if (linkedToFaculty)
            {
                throw new InvalidOperationException(
                    $"Account {student.Email} is already linked to faculty.");
            }

            await EnsureRoleAsync(userManager, user, "Student");

            student.IdentityUserId = user.Id;
        }

        foreach (var faculty in facultyMembers)
        {
            var user = await GetOrCreateUserAsync(
                userManager,
                faculty.Email,
                faculty.FirstName,
                faculty.LastName,
                initialPassword,
                resetExistingPasswords);

            if (faculty.IdentityUserId != null &&
                faculty.IdentityUserId != user.Id)
            {
                throw new InvalidOperationException(
                    $"Faculty {faculty.FacultyNumber} is linked to another account.");
            }

            var linkedToStudent = await academicContext.Students
                .AnyAsync(student => student.IdentityUserId == user.Id);

            if (linkedToStudent)
            {
                throw new InvalidOperationException(
                    $"Account {faculty.Email} is already linked to a student.");
            }

            await EnsureRoleAsync(userManager, user, "Faculty");

            faculty.IdentityUserId = user.Id;
        }

        await academicContext.SaveChangesAsync();
    }

    private static async Task<ApplicationUser> GetOrCreateUserAsync(
        UserManager<ApplicationUser> userManager,
        string email,
        string firstName,
        string lastName,
        string initialPassword,
        bool resetExistingPassword)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(
                user,
                initialPassword);

            if (!createResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    createResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Could not create account for {email}: {errors}");
            }
        }
        else if (resetExistingPassword)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);

            var resetResult = await userManager.ResetPasswordAsync(
                user,
                token,
                initialPassword);

            if (!resetResult.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    resetResult.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Could not reset password for {email}: {errors}");
            }
        }

        if (!user.IsActive)
        {
            throw new InvalidOperationException(
                $"Account {email} is inactive.");
        }

        return user;
    }

    private static async Task EnsureRoleAsync(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string expectedRole)
    {
        var roles = await userManager.GetRolesAsync(user);

        if (roles.Count > 1 ||
            (roles.Count == 1 && roles[0] != expectedRole))
        {
            throw new InvalidOperationException(
                $"Account {user.Email} has an invalid role assignment.");
        }

        if (roles.Count == 0)
        {
            var result = await userManager.AddToRoleAsync(
                user,
                expectedRole);

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Could not assign role {expectedRole}: {errors}");
            }
        }
    }
}
