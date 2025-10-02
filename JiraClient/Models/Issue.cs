using System.Text.Json.Serialization;

namespace JiraClient.Models;

/// <summary>
/// Represents a JIRA issue (work item)
/// </summary>
public class Issue
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("self")]
    public string? Self { get; set; }

    [JsonPropertyName("fields")]
    public IssueFields? Fields { get; set; }
}
