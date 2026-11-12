
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentGradeManagement.Mvc.Models.Auth;
using StudentGradeManagement.Mvc.Services;

namespace StudentGradeManagement.Mvc.Controllers;

public class AuthController : Controller
{
    private readonly ApiAuthService _apiAuthService;

    public AuthController(ApiAuthService apiAuthService)
    {
        _apiAuthService = apiAuthService;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View();
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _apiAuthService.LoginAsync(
                model.Email,
                model.Password);

            if (result is null ||
                string.IsNullOrWhiteSpace(result.AccessToken) ||
                result.Roles.Count != 1 ||
                !new[] { "Student", "Faculty" }.Contains(result.Roles[0]))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, model.Email),
                new(ClaimTypes.Email, model.Email),
                new(ClaimTypes.Role, result.Roles[0])
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var properties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = result.ExpiresAtUtc
            };

            properties.StoreTokens(
            [
                new AuthenticationToken
                {
                    Name = "access_token",
                    Value = result.AccessToken
                }
            ]);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                properties);

            return RedirectToAction("Index", "Home");
        }
        catch (HttpRequestException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Cannot connect to the API. Make sure the API is running.");
            return View(model);
        }
        catch (TaskCanceledException)
        {
            ModelState.AddModelError(
                string.Empty,
                "The API request timed out. Please try again.");
            return View(model);
        }
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
