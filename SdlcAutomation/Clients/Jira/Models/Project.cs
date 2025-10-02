using System.ComponentModel.DataAnnotations;

namespace SdlcAutomation.Clients.Jira.Models;

/// <summary>
/// Represents a JIRA project
/// </summary>
public class Project
{
    /// <summary>
    /// Project ID (read-only, populated by JIRA)
    /// </summary>
    [Display(Name = "Project ID")]
    public string? Id { get; set; }
    
    /// <summary>
    /// Project key (required for creation if specified by key)
    /// </summary>
    [RegularExpression(@"^[A-Z][A-Z0-9_]*$", ErrorMessage = "Project key must start with a letter and contain only uppercase letters, numbers, and underscores")]
    [Display(Name = "Project Key")]
    public string? Key { get; set; }
    
    /// <summary>
    /// Project name (optional, read-only)
    /// </summary>
    [Display(Name = "Project Name")]
    public string? Name { get; set; }

    /// <summary>
    /// Validates the project
    /// </summary>
    public IEnumerable<ValidationResult> Validate()
    {
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, context, results, validateAllProperties: true);
        
        // Ensure at least Key or Id is set for operations
        if (string.IsNullOrWhiteSpace(Key) && string.IsNullOrWhiteSpace(Id))
        {
            results.Add(new ValidationResult("Project must have either Key or Id specified"));
        }
        
        return results;
    }
}
