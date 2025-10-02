namespace SdlcAutomation.AzureDevOps.Models;

/// <summary>
/// Options for loading work items and related data
/// </summary>
public class WorkItemLoadOptions
{
    /// <summary>
    /// Load child work items
    /// </summary>
    public bool LoadChildren { get; set; }

    /// <summary>
    /// Load parent work items
    /// </summary>
    public bool LoadParents { get; set; }

    /// <summary>
    /// Load associated commits
    /// </summary>
    public bool LoadCommits { get; set; }

    /// <summary>
    /// Load associated pull requests
    /// </summary>
    public bool LoadPullRequests { get; set; }

    /// <summary>
    /// Default options with nothing loaded
    /// </summary>
    public static WorkItemLoadOptions Default => new();

    /// <summary>
    /// Load all available data
    /// </summary>
    public static WorkItemLoadOptions All => new()
    {
        LoadChildren = true,
        LoadParents = true,
        LoadCommits = true,
        LoadPullRequests = true
    };
}
