namespace SdlcAutomation.Clients.Jira.Models;

/// <summary>
/// Represents a JIRA issue (work item)
/// </summary>
public class Issue
{
    public string? Id { get; set; }
    public string? Key { get; set; }
    public string? Self { get; set; }
    public IssueFields? Fields { get; set; }
}
