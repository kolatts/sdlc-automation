namespace SdlcAutomation.Clients.Jira.Models;

/// <summary>
/// Represents a JIRA issue type (e.g., Story, Bug, Test, etc.)
/// </summary>
public class IssueType
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool Subtask { get; set; }
}
