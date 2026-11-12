using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace StudentGradeManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        return Ok(new
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? User.FindFirstValue("sub"),
            Email = User.FindFirstValue(ClaimTypes.Email),
            FirstName = User.FindFirstValue(ClaimTypes.GivenName),
            LastName = User.FindFirstValue(ClaimTypes.Surname),
            Roles = User.FindAll(ClaimTypes.Role)
                .Select(claim => claim.Value)
        });
    }
}