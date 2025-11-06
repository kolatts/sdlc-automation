using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;
using SdlcAutomation.GitHub.Auth;
using SdlcAutomation.GitHub.Models;

namespace SdlcAutomation.GitHub;

/// <summary>
/// Client for interacting with GitHub REST API
/// </summary>
public class GitHubApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    public GitHubApiClient(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        var authHandler = new GitHubAuthenticationHandler(token);
        _httpClient = new HttpClient(authHandler)
        {
            BaseAddress = new Uri("https://api.github.com")
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };
    }

    public static GitHubApiClient CreateFromEnvironment(string tokenEnvironmentVariable = "GITHUB_PAT")
    {
        var token = Environment.GetEnvironmentVariable(tokenEnvironmentVariable);
        
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(
                $"Environment variable '{tokenEnvironmentVariable}' is not set or is empty. " +
                "Please set it to your GitHub Personal Access Token.");
        }

        return new GitHubApiClient(token);
    }

    public async Task<Issue> CreateIssueAsync(
        string owner,
        string repository,
        Issue issue,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(owner))
            throw new ArgumentException("Owner cannot be null or empty", nameof(owner));

        if (string.IsNullOrWhiteSpace(repository))
            throw new ArgumentException("Repository cannot be null or empty", nameof(repository));

        if (issue == null)
            throw new ArgumentNullException(nameof(issue));

        // Validate the issue
        var validationResults = issue.Validate();
        if (validationResults.Any())
        {
            var errors = string.Join("; ", validationResults.Select(v => v.ErrorMessage));
            throw new ValidationException($"Issue validation failed: {errors}");
        }

        var response = await _httpClient.PostAsJsonAsync(
            $"/repos/{owner}/{repository}/issues",
            issue,
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

            var errorMessage = errorResponse?.Message ?? errorContent;

            throw new HttpRequestException(
                $"Failed to create issue. Status: {response.StatusCode}. Error: {errorMessage}");
        }

        var result = await response.Content.ReadFromJsonAsync<Issue>(_jsonOptions, cancellationToken);
        
        if (result == null)
            throw new InvalidOperationException("Failed to deserialize create issue response");

        return result;
    }

    public async Task<Issue> CreateIssueAsync(
        string owner,
        string repository,
        string title,
        string? body = null,
        List<string>? labels = null,
        List<string>? assignees = null,
        CancellationToken cancellationToken = default)
    {
        var issue = new Issue
        {
            Title = title,
            Body = body,
            Labels = labels,
            Assignees = assignees
        };

        return await CreateIssueAsync(owner, repository, issue, cancellationToken);
    }

    public async Task<Issue> GetIssueAsync(
        string owner,
        string repository,
        int issueNumber,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(owner))
            throw new ArgumentException("Owner cannot be null or empty", nameof(owner));

        if (string.IsNullOrWhiteSpace(repository))
            throw new ArgumentException("Repository cannot be null or empty", nameof(repository));

        if (issueNumber <= 0)
            throw new ArgumentException("Issue number must be greater than 0", nameof(issueNumber));

        var response = await _httpClient.GetAsync(
            $"/repos/{owner}/{repository}/issues/{issueNumber}",
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException(
                $"Failed to get issue {issueNumber}. Status: {response.StatusCode}. Error: {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<Issue>(_jsonOptions, cancellationToken);
        
        if (result == null)
            throw new InvalidOperationException($"Failed to deserialize issue {issueNumber}");

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
