using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SdlcAutomation.GitHub.Models;

/// <summary>
/// Represents a GitHub issue. Can be used for both creating and retrieving issues.
/// </summary>
public class Issue
{
    /// <summary>
    /// Issue number (read-only, populated by GitHub)
    /// </summary>
    [JsonPropertyName("number")]
    [Display(Name = "Issue Number")]
    public int? Number { get; set; }
    
    /// <summary>
    /// Issue title (required for creation)
    /// </summary>
    [JsonPropertyName("title")]
    [Required(ErrorMessage = "Title is required")]
    [MinLength(1, ErrorMessage = "Title cannot be empty")]
    [Display(Name = "Title")]
    public string Title { get; set; } = string.Empty;
    
    /// <summary>
    /// Issue body/description (optional)
    /// </summary>
    [JsonPropertyName("body")]
    [Display(Name = "Body")]
    public string? Body { get; set; }
    
    /// <summary>
    /// Issue labels (optional)
    /// </summary>
    [JsonPropertyName("labels")]
    [Display(Name = "Labels")]
    public List<string>? Labels { get; set; }
    
    /// <summary>
    /// Assignees (optional)
    /// </summary>
    [JsonPropertyName("assignees")]
    [Display(Name = "Assignees")]
    public List<string>? Assignees { get; set; }
    
    /// <summary>
    /// Milestone number (optional)
    /// </summary>
    [JsonPropertyName("milestone")]
    [Display(Name = "Milestone")]
    public int? Milestone { get; set; }
    
    /// <summary>
    /// Issue state (read-only, populated by GitHub)
    /// </summary>
    [JsonPropertyName("state")]
    [Display(Name = "State")]
    public string? State { get; set; }
    
    /// <summary>
    /// Issue HTML URL (read-only, populated by GitHub)
    /// </summary>
    [JsonPropertyName("html_url")]
    [Display(Name = "URL")]
    public string? HtmlUrl { get; set; }

    /// <summary>
    /// Validates the issue and returns validation results
    /// </summary>
    public IEnumerable<ValidationResult> Validate()
    {
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, context, results, validateAllProperties: true);
        return results;
    }

    /// <summary>
    /// Checks if the issue is valid for creation
    /// </summary>
    public bool IsValid()
    {
        return !Validate().Any();
    }
}
