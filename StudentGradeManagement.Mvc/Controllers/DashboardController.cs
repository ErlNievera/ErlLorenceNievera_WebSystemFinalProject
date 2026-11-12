
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

    public DashboardController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [Authorize(Roles = "Student")]
    public IActionResult Student()
    {
        return View();
    }

    [Authorize(Roles = "Faculty")]
    public async Task<IActionResult> Faculty()
    {
        var grades = await GetGradesAsync("api/Grades");

        if (grades is null)
            return View("ApiError");

        return View(grades);
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyGrades()
    {
        var grades = await GetGradesAsync("api/Grades/my");

        if (grades is null)
            return StatusCode(502, new
            {
                message = "Unable to retrieve your grades. Please try again."
            });

        return Ok(grades);
    }

    private async Task<List<GradeViewModel>?> GetGradesAsync(string endpoint)
    {
        var token = await HttpContext.GetTokenAsync("access_token");

        if (string.IsNullOrWhiteSpace(token))
            return null;

        var client = _httpClientFactory.CreateClient("AcademicApi");

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        using var response = await client.GetAsync(endpoint);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await HttpContext.SignOutAsync();
            return null;
        }

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content
            .ReadFromJsonAsync<List<GradeViewModel>>();
    }
    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyProfile()
    {
        var profile = await GetProfileAsync();

        if (profile is null)
            return StatusCode(502, new
            {
                message = "Unable to retrieve your student profile."
            });

        return Ok(profile);
    }

    private async Task<StudentProfileViewModel?> GetProfileAsync()
    {
        var token = await HttpContext.GetTokenAsync("access_token");

        if (string.IsNullOrWhiteSpace(token))
            return null;

        var client = _httpClientFactory.CreateClient("AcademicApi");

        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        using var response = await client.GetAsync("api/Grades/my/profile");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<StudentProfileViewModel>();
    }
}
