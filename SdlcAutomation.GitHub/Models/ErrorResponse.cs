using System.Text.Json.Serialization;

namespace SdlcAutomation.GitHub.Models;

/// <summary>
/// Represents an error response from the GitHub API
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Error message
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    
    /// <summary>
    /// Documentation URL for the error
    /// </summary>
    [JsonPropertyName("documentation_url")]
    public string? DocumentationUrl { get; set; }
    
    /// <summary>
    /// List of errors (for validation errors)
    /// </summary>
    [JsonPropertyName("errors")]
    public List<ErrorDetail>? Errors { get; set; }
}

/// <summary>
/// Represents a detailed error from the GitHub API
/// </summary>
public class ErrorDetail
{
    /// <summary>
    /// Error resource type
    /// </summary>
    [JsonPropertyName("resource")]
    public string? Resource { get; set; }
    
    /// <summary>
    /// Error field
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; set; }
    
    /// <summary>
    /// Error code
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }
    
    /// <summary>
    /// Error message
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
