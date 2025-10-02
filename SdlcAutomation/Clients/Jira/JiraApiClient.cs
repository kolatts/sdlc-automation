using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;
using SdlcAutomation.Clients.Jira.Auth;
using SdlcAutomation.Clients.Jira.Models;

namespace SdlcAutomation.Clients.Jira;

/// <summary>
/// Client for interacting with JIRA Data Center REST API
/// </summary>
public class JiraApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    public JiraApiClient(string baseUrl, string token)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("Base URL cannot be null or empty", nameof(baseUrl));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        _baseUrl = baseUrl.TrimEnd('/');
        
        var authHandler = new JiraAuthenticationHandler(token);
        _httpClient = new HttpClient(authHandler)
        {
            BaseAddress = new Uri(_baseUrl)
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    public static JiraApiClient CreateFromEnvironment(string baseUrl, string tokenEnvironmentVariable = "JIRA_PAT")
    {
        var token = Environment.GetEnvironmentVariable(tokenEnvironmentVariable);
        
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(
                $"Environment variable '{tokenEnvironmentVariable}' is not set or is empty. " +
                "Please set it to your JIRA Personal Access Token.");
        }

        return new JiraApiClient(baseUrl, token);
    }

    public async Task<Issue> CreateIssueAsync(
        Issue issue,
        CancellationToken cancellationToken = default)
    {
        if (issue == null)
            throw new ArgumentNullException(nameof(issue));

        // Validate the issue
        var validationResults = issue.Validate();
        if (validationResults.Any())
        {
            var errors = string.Join("; ", validationResults.Select(v => v.ErrorMessage));
            throw new ValidationException($"Issue validation failed: {errors}");
        }

        // Create request payload with just the fields
        var requestPayload = new { fields = issue.Fields };

        var response = await _httpClient.PostAsJsonAsync(
            "/rest/api/2/issue",
            requestPayload,
            _jsonOptions,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            ErrorResponse? errorResponse = null;

            try
            {
                errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorContent, _jsonOptions);
            }
            catch
            {
                // If we can't parse the error response, just use the raw content
            }

            var errorMessage = errorResponse?.ErrorMessages != null && errorResponse.ErrorMessages.Count > 0
                ? string.Join("; ", errorResponse.ErrorMessages)
                : errorContent;

            throw new HttpRequestException(
                $"Failed to create issue. Status: {response.StatusCode}. Error: {errorMessage}");
        }

        var result = await response.Content.ReadFromJsonAsync<Issue>(_jsonOptions, cancellationToken);
        
        if (result == null)
            throw new InvalidOperationException("Failed to deserialize create issue response");

        return result;
    }

    public async Task<Issue> CreateIssueAsync(
        string projectKey,
        string issueTypeName,
        string summary,
        string? description = null,
        Dictionary<string, object>? additionalFields = null,
        CancellationToken cancellationToken = default)
    {
        var issue = new Issue
        {
            Fields = new IssueFields
            {
                Project = new Project { Key = projectKey },
                IssueType = new IssueType { Name = issueTypeName },
                Summary = summary,
                Description = description,
                CustomFields = additionalFields
            }
        };

        return await CreateIssueAsync(issue, cancellationToken);
    }

    public async Task<Issue> CreateUserStoryAsync(
        string projectKey,
        string summary,
        string? description = null,
        Dictionary<string, object>? additionalFields = null,
        CancellationToken cancellationToken = default)
    {
        return await CreateIssueAsync(projectKey, "Story", summary, description, additionalFields, cancellationToken);
    }

    public async Task<Issue> CreateTestItemAsync(
        string projectKey,
        string summary,
        string? description = null,
        Dictionary<string, object>? additionalFields = null,
        CancellationToken cancellationToken = default)
    {
        return await CreateIssueAsync(projectKey, "Test", summary, description, additionalFields, cancellationToken);
    }

    public async Task<Issue> GetIssueAsync(string issueKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(issueKey))
            throw new ArgumentException("Issue key cannot be null or empty", nameof(issueKey));

        var response = await _httpClient.GetAsync($"/rest/api/2/issue/{issueKey}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Failed to get issue {issueKey}. Status: {response.StatusCode}. Error: {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<Issue>(_jsonOptions, cancellationToken);
        
        if (result == null)
            throw new InvalidOperationException($"Failed to deserialize issue {issueKey}");

        return result;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient?.Dispose();
            _disposed = true;
        }
    }
}
