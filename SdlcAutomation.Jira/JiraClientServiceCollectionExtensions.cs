using Microsoft.Extensions.DependencyInjection;

namespace SdlcAutomation.Jira;

/// <summary>
/// Extension methods for adding JIRA client to IServiceCollection
/// </summary>
public static class JiraClientServiceCollectionExtensions
{
    /// <summary>
    /// Adds JIRA client services to the service collection using environment variables
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddJiraClient(this IServiceCollection services)
    {
        return services.AddJiraClientFromEnvironment();
    }

    /// <summary>
    /// Adds JIRA client services to the service collection with explicit values
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="baseUrl">The base URL of the JIRA instance</param>
    /// <param name="token">The Personal Access Token for authentication</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddJiraClient(
        this IServiceCollection services,
        string baseUrl,
        string token)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException("Base URL cannot be null or empty", nameof(baseUrl));

        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token cannot be null or empty", nameof(token));

        services.AddSingleton<JiraApiClient>(provider => 
            new JiraApiClient(baseUrl, token));
        
        return services;
    }

    /// <summary>
    /// Adds JIRA client services to the service collection using environment variables
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="baseUrlEnvironmentVariable">Name of the environment variable containing the base URL (defaults to "JIRA_BASE_URL")</param>
    /// <param name="tokenEnvironmentVariable">Name of the environment variable containing the PAT (defaults to "JIRA_PAT")</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddJiraClientFromEnvironment(
        this IServiceCollection services,
        string baseUrlEnvironmentVariable = "JIRA_BASE_URL",
        string tokenEnvironmentVariable = "JIRA_PAT")
    {
        var baseUrl = Environment.GetEnvironmentVariable(baseUrlEnvironmentVariable);
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            throw new InvalidOperationException(
                $"Environment variable '{baseUrlEnvironmentVariable}' is not set or is empty. " +
                "Please set it to your JIRA instance URL.");
        }

        var token = Environment.GetEnvironmentVariable(tokenEnvironmentVariable);
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException(
                $"Environment variable '{tokenEnvironmentVariable}' is not set or is empty. " +
                "Please set it to your JIRA Personal Access Token.");
        }

        return services.AddJiraClient(baseUrl, token);
    }
}

