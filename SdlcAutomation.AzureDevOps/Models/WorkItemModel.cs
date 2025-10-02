namespace SdlcAutomation.AzureDevOps.Models;

/// <summary>
/// Simplified model for Azure DevOps work items
/// </summary>
public class WorkItemModel
{
    /// <summary>
    /// Work item ID
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Work item title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Work item description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Acceptance criteria
    /// </summary>
    public string? AcceptanceCriteria { get; set; }

    /// <summary>
    /// Person assigned to the work item
    /// </summary>
    public string? AssignedTo { get; set; }

    /// <summary>
    /// Work item type (e.g., Feature, Epic, Story, Task)
    /// </summary>
    public string? WorkItemType { get; set; }

    /// <summary>
    /// Current state of the work item
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Date the work item was created
    /// </summary>
    public DateTime? CreatedDate { get; set; }

    /// <summary>
    /// Date the work item was last changed
    /// </summary>
    public DateTime? ChangedDate { get; set; }

    /// <summary>
    /// Date the work item was closed
    /// </summary>
    public DateTime? ClosedDate { get; set; }

    /// <summary>
    /// Date the work item was resolved
    /// </summary>
    public DateTime? ResolvedDate { get; set; }

    /// <summary>
    /// Date the work item was activated
    /// </summary>
    public DateTime? ActivatedDate { get; set; }

    /// <summary>
    /// Date the work item state changed
    /// </summary>
    public DateTime? StateChangeDate { get; set; }

    /// <summary>
    /// Child work items (loaded when requested)
    /// </summary>
    public List<WorkItemModel>? Children { get; set; }

    /// <summary>
    /// Parent work items (loaded when requested)
    /// </summary>
    public List<WorkItemModel>? Parents { get; set; }

    /// <summary>
    /// Associated commits (loaded when requested)
    /// </summary>
    public List<CommitInfo>? Commits { get; set; }

    /// <summary>
    /// Associated pull requests (loaded when requested)
    /// </summary>
    public List<PullRequestInfo>? PullRequests { get; set; }
}

/// <summary>
/// Information about a commit associated with a work item
/// </summary>
public class CommitInfo
{
    /// <summary>
    /// Commit ID
    /// </summary>
    public string? CommitId { get; set; }

    /// <summary>
    /// Commit message
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// Commit author
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Date the commit was created
    /// </summary>
    public DateTime? CommitDate { get; set; }

    /// <summary>
    /// Remote URL for the commit
    /// </summary>
    public string? RemoteUrl { get; set; }
}

/// <summary>
/// Information about a pull request associated with a work item
/// </summary>
public class PullRequestInfo
{
    /// <summary>
    /// Pull request ID
    /// </summary>
    public int PullRequestId { get; set; }

    /// <summary>
    /// Pull request title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Pull request description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Pull request status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Pull request creator
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Date the pull request was created
    /// </summary>
    public DateTime? CreationDate { get; set; }

    /// <summary>
    /// Date the pull request was closed
    /// </summary>
    public DateTime? ClosedDate { get; set; }

    /// <summary>
    /// Source branch
    /// </summary>
    public string? SourceBranch { get; set; }

    /// <summary>
    /// Target branch
    /// </summary>
    public string? TargetBranch { get; set; }

    /// <summary>
    /// Remote URL for the pull request
    /// </summary>
    public string? RemoteUrl { get; set; }
}
