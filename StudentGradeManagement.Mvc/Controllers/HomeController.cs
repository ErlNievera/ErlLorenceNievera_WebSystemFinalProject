
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudentGradeManagement.Mvc.Controllers;

public class HomeController : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        // Redirect unauthenticated users to Login.
        if (User.Identity?.IsAuthenticated != true)
        {
            return RedirectToAction("Login", "Auth");
        }

        // Redirect students to their dashboard.
        if (User.IsInRole("Student"))
        {
            return RedirectToAction("Student", "Dashboard");
        }

        // Redirect faculty to their dashboard.
        if (User.IsInRole("Faculty"))
        {
            return RedirectToAction("Faculty", "Dashboard");
        }

        // Authenticated user with an unrecognized role.
        return StatusCode(403);
    }

    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }
}
