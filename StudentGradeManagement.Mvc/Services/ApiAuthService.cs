
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace StudentGradeManagement.Mvc.Services;

public class ApiAuthService
{
    private readonly HttpClient _httpClient;

    public ApiAuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiLoginResponse?> LoginAsync(
        string email,
        string password)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/Auth/login",
            new
            {
                Email = email,
                Password = password
            });

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ApiLoginResponse>();
    }
}

public class ApiLoginResponse
{
    [JsonPropertyName("accessToken")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("tokenType")]
    public string TokenType { get; set; } = "Bearer";

    [JsonPropertyName("expiresAtUtc")]
    public DateTime ExpiresAtUtc { get; set; }

    [JsonPropertyName("roles")]
    public List<string> Roles { get; set; } = [];
}
