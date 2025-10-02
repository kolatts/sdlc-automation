using System.Text.Json.Serialization;

namespace SdlcAutomation.Clients.Jira.Models;

/// <summary>
/// Represents the fields of a JIRA issue
/// </summary>
public class IssueFields
{
    public Project? Project { get; set; }
    public string? Summary { get; set; }
    public string? Description { get; set; }
    
    [JsonPropertyName("issuetype")]
    public IssueType? IssueType { get; set; }
    
    public User? Assignee { get; set; }
    public User? Reporter { get; set; }
    public Priority? Priority { get; set; }
    public List<string>? Labels { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object>? CustomFields { get; set; }
}
