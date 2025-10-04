using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SdlcAutomation.Jira.Models;

/// <summary>
/// Represents the fields of a JIRA issue. Can be used for both creating and retrieving issues.
/// </summary>
public class IssueFields
{
    /// <summary>
    /// Project information (required for creation)
    /// </summary>
    [Required(ErrorMessage = "Project is required")]
    [Display(Name = "Project")]
    public Project Project { get; set; } = new Project();
    
    /// <summary>
    /// Issue summary/title (required for creation)
    /// </summary>
    [Required(ErrorMessage = "Summary is required")]
    [MinLength(5, ErrorMessage = "Summary must be at least 5 characters long")]
    [MaxLength(255, ErrorMessage = "Summary cannot exceed 255 characters")]
    [Display(Name = "Summary")]
    public string Summary { get; set; } = string.Empty;
    
    /// <summary>
    /// Issue description (optional)
    /// </summary>
    [MaxLength(32767, ErrorMessage = "Description cannot exceed 32767 characters")]
    [Display(Name = "Description")]
    public string? Description { get; set; }
    
    /// <summary>
    /// Issue type (required for creation)
    /// </summary>
    [JsonPropertyName("issuetype")]
    [Required(ErrorMessage = "Issue type is required")]
    [Display(Name = "Issue Type")]
    public IssueType IssueType { get; set; } = new IssueType();
    
    /// <summary>
    /// Assignee (optional)
    /// </summary>
    [Display(Name = "Assignee")]
    public User? Assignee { get; set; }
    
    /// <summary>
    /// Reporter (optional)
    /// </summary>
    [Display(Name = "Reporter")]
    public User? Reporter { get; set; }
    
    /// <summary>
    /// Priority (optional)
    /// </summary>
    [Display(Name = "Priority")]
    public Priority? Priority { get; set; }
    
    /// <summary>
    /// Labels (optional)
    /// </summary>
    [Display(Name = "Labels")]
    public List<string>? Labels { get; set; }

    /// <summary>
    /// Custom fields (optional)
    /// </summary>
    [JsonExtensionData]
    [Display(Name = "Custom Fields")]
    public Dictionary<string, object>? CustomFields { get; set; }

    /// <summary>
    /// Validates the fields and returns validation results
    /// </summary>
    public IEnumerable<ValidationResult> Validate()
    {
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, context, results, validateAllProperties: true);
        
        // Validate nested Project
        if (Project != null)
        {
            var projectResults = Project.Validate();
            results.AddRange(projectResults);
        }
        
        // Validate nested IssueType
        if (IssueType != null)
        {
            var issueTypeResults = IssueType.Validate();
            results.AddRange(issueTypeResults);
        }
        
        return results;
    }

    /// <summary>
    /// Checks if the fields are valid
    /// </summary>
    public bool IsValid()
    {
        return !Validate().Any();
    }
}
