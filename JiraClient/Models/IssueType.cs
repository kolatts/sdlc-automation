using System.Text.Json.Serialization;

namespace JiraClient.Models;

/// <summary>
/// Represents a JIRA issue type (e.g., Story, Bug, Test, etc.)
/// </summary>
public class IssueType
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("subtask")]
    public bool Subtask { get; set; }
}
