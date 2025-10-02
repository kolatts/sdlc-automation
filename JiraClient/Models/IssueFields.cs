using System.Text.Json.Serialization;

namespace JiraClient.Models;

/// <summary>
/// Represents the fields of a JIRA issue
/// </summary>
public class IssueFields
{
    [JsonPropertyName("project")]
    public Project? Project { get; set; }

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("issuetype")]
    public IssueType? IssueType { get; set; }

    [JsonPropertyName("assignee")]
    public User? Assignee { get; set; }

    [JsonPropertyName("reporter")]
    public User? Reporter { get; set; }

    [JsonPropertyName("priority")]
    public Priority? Priority { get; set; }

    [JsonPropertyName("labels")]
    public List<string>? Labels { get; set; }

    // Support for custom fields using dictionary
    [JsonExtensionData]
    public Dictionary<string, object>? CustomFields { get; set; }
}
