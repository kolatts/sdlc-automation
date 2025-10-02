namespace SdlcAutomation.Clients.Jira.Models;

/// <summary>
/// Response model for creating a JIRA issue
/// </summary>
public class CreateIssueResponse
{
    public string? Id { get; set; }
    public string? Key { get; set; }
    public string? Self { get; set; }
}
