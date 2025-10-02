using System.ComponentModel.DataAnnotations;

namespace SdlcAutomation.Clients.Jira.Models;

/// <summary>
/// Request model for creating a JIRA issue via command line
/// </summary>
public class CreateIssueCommandRequest
{
    /// <summary>
    /// The JIRA project key (e.g., PROJ)
    /// </summary>
    [Required(ErrorMessage = "Project key is required")]
    [RegularExpression(@"^[A-Z][A-Z0-9_]*$", ErrorMessage = "Project key must start with a letter and contain only uppercase letters, numbers, and underscores")]
    public string ProjectKey { get; set; } = string.Empty;

    /// <summary>
    /// The issue type (Story, Test, Bug, etc.)
    /// </summary>
    [Required(ErrorMessage = "Issue type is required")]
    [MinLength(1, ErrorMessage = "Issue type cannot be empty")]
    public string IssueType { get; set; } = "Story";

    /// <summary>
    /// The issue summary/title
    /// </summary>
    [Required(ErrorMessage = "Summary is required")]
    [MinLength(5, ErrorMessage = "Summary must be at least 5 characters long")]
    [MaxLength(255, ErrorMessage = "Summary cannot exceed 255 characters")]
    public string Summary { get; set; } = string.Empty;

    /// <summary>
    /// The issue description
    /// </summary>
    [MaxLength(32767, ErrorMessage = "Description cannot exceed 32767 characters")]
    public string? Description { get; set; }

    /// <summary>
    /// Validates the request and returns validation results
    /// </summary>
    /// <returns>Collection of validation results</returns>
    public IEnumerable<ValidationResult> Validate()
    {
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, context, results, validateAllProperties: true);
        return results;
    }

    /// <summary>
    /// Checks if the request is valid
    /// </summary>
    /// <returns>True if valid, false otherwise</returns>
    public bool IsValid()
    {
        return !Validate().Any();
    }
}
