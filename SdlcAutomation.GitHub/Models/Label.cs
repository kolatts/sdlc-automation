using System.Text.Json.Serialization;

namespace SdlcAutomation.GitHub.Models;

/// <summary>
/// Represents a GitHub label
/// </summary>
public class Label
{
    /// <summary>
    /// Label name
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
    
    /// <summary>
    /// Label color (hex code without #)
    /// </summary>
    [JsonPropertyName("color")]
    public string? Color { get; set; }
    
    /// <summary>
    /// Label description
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
