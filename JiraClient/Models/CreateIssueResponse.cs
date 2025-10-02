using System.Text.Json.Serialization;

namespace JiraClient.Models;

/// <summary>
/// Response model for creating a JIRA issue
/// </summary>
public class CreateIssueResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("self")]
    public string? Self { get; set; }
}
