using System.Text.Json.Serialization;

namespace JiraClient.Models;

/// <summary>
/// Represents a JIRA project
/// </summary>
public class Project
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
