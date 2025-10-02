namespace SdlcAutomation.Clients.Jira.Models;

/// <summary>
/// Represents a JIRA API error response
/// </summary>
public class ErrorResponse
{
    public List<string>? ErrorMessages { get; set; }
    public Dictionary<string, string>? Errors { get; set; }
}
