namespace SdlcAutomation.Models;

/// <summary>
/// Represents an organization with its associated configurations
/// </summary>
public class OrganizationConfig
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public GitHubConfig? GitHub { get; set; }
    public JiraConfig? Jira { get; set; }
    public AzureDevOpsConfig? AzureDevOps { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// GitHub configuration for an organization
/// </summary>
public class GitHubConfig
{
    public string? Organization { get; set; }
    public string? BaseUrl { get; set; }
    public string PatEnvironmentVariable { get; set; } = string.Empty;
}

/// <summary>
/// Jira configuration for an organization
/// </summary>
public class JiraConfig
{
    public string? BaseUrl { get; set; }
    public string PatEnvironmentVariable { get; set; } = string.Empty;
}

/// <summary>
/// Azure DevOps configuration for an organization
/// </summary>
public class AzureDevOpsConfig
{
    public string? OrganizationUrl { get; set; }
    public string? DefaultProject { get; set; }
    public string PatEnvironmentVariable { get; set; } = string.Empty;
}
