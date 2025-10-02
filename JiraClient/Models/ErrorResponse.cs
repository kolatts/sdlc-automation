using System.Text.Json.Serialization;

namespace JiraClient.Models;

/// <summary>
/// Represents a JIRA API error response
/// </summary>
public class ErrorResponse
{
    [JsonPropertyName("errorMessages")]
    public List<string>? ErrorMessages { get; set; }

    [JsonPropertyName("errors")]
    public Dictionary<string, string>? Errors { get; set; }
}
