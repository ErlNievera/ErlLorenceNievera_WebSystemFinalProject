using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudentGradeManagement.Mvc.Controllers;

public class HomeController : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated != true)
            return RedirectToAction("Login", "Auth");

        if (User.IsInRole("Student"))
            return RedirectToAction("Student", "Dashboard");

        if (User.IsInRole("Faculty"))
            return RedirectToAction("Faculty", "Dashboard");

        return Forbid();
    }

    public IActionResult Privacy()
    {
        return View();
    }
}