
using Microsoft.AspNetCore.Identity;
using StudentGradeManagement.Domain.Constants;

namespace StudentGradeManagement.Infrastructure.Identity;

public static class RoleSeeder
{
    public static async Task SeedRolesAsync(
        RoleManager<IdentityRole> roleManager)
    {
        foreach (var roleName in ApplicationRoles.All)
        {
            if (await roleManager.RoleExistsAsync(roleName))
            {
                continue;
            }

            var result = await roleManager.CreateAsync(
                new IdentityRole(roleName));

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    "; ",
                    result.Errors.Select(error => error.Description));

                throw new InvalidOperationException(
                    $"Could not create role '{roleName}': {errors}");
            }
        }
    }
}
