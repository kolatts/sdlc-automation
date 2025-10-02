using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using JiraClient.Auth;
using JiraClient.Models;

namespace JiraClient;

/// <summary>
/// Client for interacting with JIRA Data Center REST API
/// </summary>
public class JiraApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    /// <summary>
    /// Creates a new JIRA API client
    /// </summary>
    /// <param name="baseUrl">Base URL of the JIRA instance (e.g., https://jira.example.com)</param>
    /// <param name="token">Personal Access Token for authentication</param>
    public JiraApiClient(string baseUrl, string token)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new ArgumentException("Base URL cannot be null or empty", nameof(baseUrl));
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new ArgumentException("Token cannot be null or empty", nameof(token));
        }

        _baseUrl = baseUrl.TrimEnd('/');
        
        var authHandler = new JiraAuthenticationHandler(token);
        _httpClient = new HttpClient(authHandler)
        {
            BaseAddress = new Uri(_baseUrl)
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    /// <summary>
    /// Creates a JIRA API client using environment variable for token
    /// </summary>
    /// <param name="baseUrl">Base URL of the JIRA instance</param>
    /// <param name="tokenEnvironmentVariable">Name of the environment variable containing the PAT (defaults to "JIRA_PAT")</param>
    /// <returns>Configured JiraApiClient</returns>
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

    /// <summary>
    /// Creates a new issue in JIRA
    /// </summary>
    /// <param name="projectKey">The project key where the issue will be created</param>
    /// <param name="issueTypeName">The type of issue to create (e.g., "Story", "Test", "Bug")</param>
    /// <param name="summary">The summary/title of the issue</param>
    /// <param name="description">The description of the issue</param>
    /// <param name="additionalFields">Optional additional fields to set on the issue</param>
    /// <returns>The created issue response</returns>
    public async Task<CreateIssueResponse> CreateIssueAsync(
        string projectKey,
        string issueTypeName,
        string summary,
        string? description = null,
        Dictionary<string, object>? additionalFields = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(projectKey))
        {
            throw new ArgumentException("Project key cannot be null or empty", nameof(projectKey));
        }

        if (string.IsNullOrWhiteSpace(issueTypeName))
        {
            throw new ArgumentException("Issue type name cannot be null or empty", nameof(issueTypeName));
        }

        if (string.IsNullOrWhiteSpace(summary))
        {
            throw new ArgumentException("Summary cannot be null or empty", nameof(summary));
        }

        var fields = new IssueFields
        {
            Project = new Project { Key = projectKey },
            IssueType = new IssueType { Name = issueTypeName },
            Summary = summary,
            Description = description
        };

        // Add any additional custom fields
        if (additionalFields != null && additionalFields.Count > 0)
        {
            fields.CustomFields = new Dictionary<string, object>(additionalFields);
        }

        var request = new CreateIssueRequest(fields);

        var response = await _httpClient.PostAsJsonAsync(
            "/rest/api/2/issue",
            request,
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

        var result = await response.Content.ReadFromJsonAsync<CreateIssueResponse>(_jsonOptions, cancellationToken);
        
        if (result == null)
        {
            throw new InvalidOperationException("Failed to deserialize create issue response");
        }

        return result;
    }

    /// <summary>
    /// Creates a user story in JIRA
    /// </summary>
    /// <param name="projectKey">The project key where the story will be created</param>
    /// <param name="summary">The summary/title of the story</param>
    /// <param name="description">The description of the story</param>
    /// <param name="additionalFields">Optional additional fields to set on the story</param>
    /// <returns>The created issue response</returns>
    public async Task<CreateIssueResponse> CreateUserStoryAsync(
        string projectKey,
        string summary,
        string? description = null,
        Dictionary<string, object>? additionalFields = null,
        CancellationToken cancellationToken = default)
    {
        return await CreateIssueAsync(projectKey, "Story", summary, description, additionalFields, cancellationToken);
    }

    /// <summary>
    /// Creates a test-related work item in JIRA
    /// </summary>
    /// <param name="projectKey">The project key where the test item will be created</param>
    /// <param name="summary">The summary/title of the test item</param>
    /// <param name="description">The description of the test item</param>
    /// <param name="additionalFields">Optional additional fields to set on the test item</param>
    /// <returns>The created issue response</returns>
    public async Task<CreateIssueResponse> CreateTestItemAsync(
        string projectKey,
        string summary,
        string? description = null,
        Dictionary<string, object>? additionalFields = null,
        CancellationToken cancellationToken = default)
    {
        return await CreateIssueAsync(projectKey, "Test", summary, description, additionalFields, cancellationToken);
    }

    /// <summary>
    /// Gets an issue by key
    /// </summary>
    /// <param name="issueKey">The issue key (e.g., "PROJ-123")</param>
    /// <returns>The issue</returns>
    public async Task<Issue> GetIssueAsync(string issueKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(issueKey))
        {
            throw new ArgumentException("Issue key cannot be null or empty", nameof(issueKey));
        }

        var response = await _httpClient.GetAsync($"/rest/api/2/issue/{issueKey}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Failed to get issue {issueKey}. Status: {response.StatusCode}. Error: {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<Issue>(_jsonOptions, cancellationToken);
        
        if (result == null)
        {
            throw new InvalidOperationException($"Failed to deserialize issue {issueKey}");
        }

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
