
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentGradeManagement.Mvc.Models.Grades;

namespace StudentGradeManagement.Mvc.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IHttpClientFactory httpClientFactory,
        ILogger<DashboardController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    // ==========================================
    // STUDENT DASHBOARD
    // ==========================================

    [Authorize(Roles = "Student")]
    public IActionResult Student()
    {
        return View();
    }

    // ==========================================
    // FACULTY DASHBOARD
    // ==========================================

    [Authorize(Roles = "Faculty")]
    public async Task<IActionResult> Faculty()
    {
        var grades = await GetGradesAsync("api/Grades");

        if (grades is null)
        {
            return View("ApiError");
        }

        return View(grades);
    }

    // ==========================================
    // STUDENT GRADES API PROXY
    // ==========================================

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyGrades()
    {
        var grades = await GetGradesAsync("api/Grades/my");

        if (grades is null)
        {
            return StatusCode(502, new
            {
                message = "Unable to retrieve your grades. Please try again."
            });
        }

        return Ok(grades);
    }

    // ==========================================
    // GET GRADES HELPER
    // ==========================================

    private async Task<List<GradeViewModel>?> GetGradesAsync(
        string endpoint)
    {
        var token = await HttpContext.GetTokenAsync("access_token");

        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning(
                "Grade request failed for {User}. JWT access token is missing.",
                User.Identity?.Name);

            return null;
        }

        var client = _httpClientFactory.CreateClient("AcademicApi");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        try
        {
            using var response = await client.GetAsync(endpoint);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning(
                    "Grade API returned 401 for {User}. Signing out.",
                    User.Identity?.Name);

                await HttpContext.SignOutAsync();

                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent =
                    await response.Content.ReadAsStringAsync();

                _logger.LogError(
                    "Grade API request failed. User: {User}, Endpoint: {Endpoint}, Status: {StatusCode}, Response: {Response}",
                    User.Identity?.Name,
                    endpoint,
                    (int)response.StatusCode,
                    errorContent);

                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<List<GradeViewModel>>();
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(
                exception,
                "Unable to connect to Academic API. Endpoint: {Endpoint}",
                endpoint);

            return null;
        }
        catch (System.Text.Json.JsonException exception)
        {
            _logger.LogError(
                exception,
                "Unable to deserialize grade API response. Endpoint: {Endpoint}",
                endpoint);

            return null;
        }
    }

    // ==========================================
    // STUDENT PROFILE API PROXY
    // ==========================================

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyProfile()
    {
        var profile = await GetProfileAsync();

        if (profile is null)
        {
            return StatusCode(502, new
            {
                message = "Unable to retrieve your student profile."
            });
        }

        return Ok(profile);
    }

    // ==========================================
    // GET STUDENT PROFILE HELPER
    // ==========================================

    private async Task<StudentProfileViewModel?> GetProfileAsync()
    {
        var token = await HttpContext.GetTokenAsync("access_token");

        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogWarning(
                "Student profile request failed for {User}: JWT access token is missing.",
                User.Identity?.Name);

            return null;
        }

        var client = _httpClientFactory.CreateClient("AcademicApi");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        try
        {
            using var response = await client.GetAsync(
                "api/Grades/my/profile");

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                _logger.LogWarning(
                    "Student profile API returned 401 for {User}. Signing out.",
                    User.Identity?.Name);

                await HttpContext.SignOutAsync();

                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorContent =
                    await response.Content.ReadAsStringAsync();

                _logger.LogError(
                    "Student profile API request failed. User: {User}, Status: {StatusCode}, Reason: {ReasonPhrase}, Response: {Response}",
                    User.Identity?.Name,
                    (int)response.StatusCode,
                    response.ReasonPhrase,
                    errorContent);

                return null;
            }

            return await response.Content
                .ReadFromJsonAsync<StudentProfileViewModel>();
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(
                exception,
                "Unable to connect to Academic API while retrieving student profile for {User}.",
                User.Identity?.Name);

            return null;
        }
        catch (System.Text.Json.JsonException exception)
        {
            _logger.LogError(
                exception,
                "Unable to deserialize student profile response for {User}.",
                User.Identity?.Name);

            return null;
        }
    }
}
