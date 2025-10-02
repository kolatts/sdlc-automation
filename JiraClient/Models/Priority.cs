using System.Text.Json.Serialization;

namespace JiraClient.Models;

/// <summary>
/// Represents a JIRA priority
/// </summary>
public class Priority
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
