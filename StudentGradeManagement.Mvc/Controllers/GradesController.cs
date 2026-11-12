using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentGradeManagement.Mvc.Models.Grades;

namespace StudentGradeManagement.Mvc.Controllers;

[Authorize(Roles = "Faculty")]
public class GradesController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public GradesController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private async Task<HttpClient?> GetApiClientAsync()
    {
        var token = await HttpContext.GetTokenAsync("access_token");

        if (string.IsNullOrWhiteSpace(token))
            return null;

        var client = _httpClientFactory.CreateClient("AcademicApi");
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        return client;
    }

    public async Task<IActionResult> Index()
    {
        using var client = await GetApiClientAsync();

        if (client is null)
            return RedirectToAction("Login", "Auth");

        using var response = await client.GetAsync("api/Grades");

        if (!response.IsSuccessStatusCode)
            return View("~/Views/Dashboard/ApiError.cshtml");

        var grades = await response.Content
            .ReadFromJsonAsync<List<GradeViewModel>>() ?? [];

        return View(grades);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new GradeFormViewModel();

        if (!await LoadStudentsAsync(model))
            return View("~/Views/Dashboard/ApiError.cshtml");

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GradeFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadStudentsAsync(model);
            return View(model);
        }

        using var client = await GetApiClientAsync();

        if (client is null)
            return RedirectToAction("Login", "Auth");

        var request = new
        {
            model.StudentId,
            model.SubjectCode,
            model.SubjectName,
            model.Grade,
            model.Semester,
            model.SchoolYear
        };

        using var response = await client.PostAsJsonAsync(
            "api/Grades", request);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            ModelState.AddModelError(
                string.Empty,
                "This student already has a grade for this subject, semester, and school year.");
            await LoadStudentsAsync(model);
            return View(model);
        }

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(
                string.Empty,
                await GetApiErrorAsync(response));
            await LoadStudentsAsync(model);
            return View(model);
        }

        TempData["Success"] = "Grade created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        using var client = await GetApiClientAsync();

        if (client is null)
            return RedirectToAction("Login", "Auth");

        using var response = await client.GetAsync($"api/Grades/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return NotFound();

        if (!response.IsSuccessStatusCode)
            return View("~/Views/Dashboard/ApiError.cshtml");

        var grade = await response.Content.ReadFromJsonAsync<GradeViewModel>();

        if (grade is null)
            return NotFound();

        var model = new GradeFormViewModel
        {
            GradeId = grade.GradeId,
            StudentId = grade.StudentId,
            SubjectCode = grade.SubjectCode,
            SubjectName = grade.SubjectName,
            Grade = grade.Grade,
            Semester = grade.Semester,
            SchoolYear = grade.SchoolYear
        };

        if (!await LoadStudentsAsync(model))
            return View("~/Views/Dashboard/ApiError.cshtml");

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        GradeFormViewModel model)
    {
        if (id != model.GradeId)
            return BadRequest();

        if (!ModelState.IsValid)
        {
            await LoadStudentsAsync(model);
            return View(model);
        }

        using var client = await GetApiClientAsync();

        if (client is null)
            return RedirectToAction("Login", "Auth");

        var request = new
        {
            model.StudentId,
            model.SubjectCode,
            model.SubjectName,
            model.Grade,
            model.Semester,
            model.SchoolYear
        };

        using var response = await client.PutAsJsonAsync(
            $"api/Grades/{id}", request);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            ModelState.AddModelError(
                string.Empty,
                "A grade already exists for this student, subject, semester, and school year.");
            await LoadStudentsAsync(model);
            return View(model);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
            return NotFound();

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(
                string.Empty,
                await GetApiErrorAsync(response));
            await LoadStudentsAsync(model);
            return View(model);
        }

        TempData["Success"] = "Grade updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        using var client = await GetApiClientAsync();

        if (client is null)
            return RedirectToAction("Login", "Auth");

        using var response = await client.DeleteAsync($"api/Grades/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return NotFound();

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = await GetApiErrorAsync(response);
            return RedirectToAction(nameof(Index));
        }

        TempData["Success"] = "Grade deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> LoadStudentsAsync(GradeFormViewModel model)
    {
        using var client = await GetApiClientAsync();

        if (client is null)
            return false;

        using var response = await client.GetAsync("api/Grades/students");

        if (!response.IsSuccessStatusCode)
            return false;

        model.Students = await response.Content
            .ReadFromJsonAsync<List<StudentOptionViewModel>>() ?? [];

        return true;
    }

    private static async Task<string> GetApiErrorAsync(
        HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();

        try
        {
            using var document = JsonDocument.Parse(body);

            if (document.RootElement.TryGetProperty(
                "message", out var message))
                return message.GetString() ?? "The API request failed.";
        }
        catch (JsonException)
        {
            // Use the generic message below.
        }

        return "The request could not be completed. Please try again.";
    }
}