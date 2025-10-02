using System.ComponentModel.DataAnnotations;

namespace SdlcAutomation.Clients.Jira.Models;

/// <summary>
/// Represents a JIRA issue (work item). Can be used for both creating and retrieving issues.
/// </summary>
public class Issue
{
    /// <summary>
    /// Issue ID (read-only, populated by JIRA)
    /// </summary>
    public string? Id { get; set; }
    
    /// <summary>
    /// Issue key (read-only, populated by JIRA)
    /// </summary>
    public string? Key { get; set; }
    
    /// <summary>
    /// Issue URL (read-only, populated by JIRA)
    /// </summary>
    public string? Self { get; set; }
    
    /// <summary>
    /// Issue fields (required for creation, populated for retrieval)
    /// </summary>
    [Required(ErrorMessage = "Fields are required")]
    public IssueFields Fields { get; set; } = new IssueFields();

    /// <summary>
    /// Validates the issue and returns validation results
    /// </summary>
    public IEnumerable<ValidationResult> Validate()
    {
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, context, results, validateAllProperties: true);
        
        // Also validate nested Fields
        if (Fields != null)
        {
            var fieldsResults = Fields.Validate();
            results.AddRange(fieldsResults);
        }
        
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
