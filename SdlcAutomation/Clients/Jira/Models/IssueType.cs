using System.ComponentModel.DataAnnotations;

namespace SdlcAutomation.Clients.Jira.Models;

/// <summary>
/// Represents a JIRA issue type (e.g., Story, Bug, Test, etc.)
/// </summary>
public class IssueType
{
    /// <summary>
    /// Issue type ID (read-only, populated by JIRA)
    /// </summary>
    public string? Id { get; set; }
    
    /// <summary>
    /// Issue type name (required for creation if specified by name)
    /// </summary>
    [MinLength(1, ErrorMessage = "Issue type name cannot be empty")]
    public string? Name { get; set; }
    
    /// <summary>
    /// Issue type description (read-only)
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// Whether this is a subtask type (read-only)
    /// </summary>
    public bool Subtask { get; set; }

    /// <summary>
    /// Validates the issue type
    /// </summary>
    public IEnumerable<ValidationResult> Validate()
    {
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, context, results, validateAllProperties: true);
        
        // Ensure at least Name or Id is set for operations
        if (string.IsNullOrWhiteSpace(Name) && string.IsNullOrWhiteSpace(Id))
        {
            results.Add(new ValidationResult("Issue type must have either Name or Id specified"));
        }
        
        return results;
    }
}
