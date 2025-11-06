using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SdlcAutomation.GitHub.Models;

/// <summary>
/// Represents a GitHub repository
/// </summary>
public class Repository
{
    /// <summary>
    /// Repository owner (username or organization)
    /// </summary>
    [Required(ErrorMessage = "Repository owner is required")]
    [Display(Name = "Owner")]
    public string Owner { get; set; } = string.Empty;
    
    /// <summary>
    /// Repository name
    /// </summary>
    [Required(ErrorMessage = "Repository name is required")]
    [Display(Name = "Repository")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Validates the repository
    /// </summary>
    public IEnumerable<ValidationResult> Validate()
    {
        var context = new ValidationContext(this);
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(this, context, results, validateAllProperties: true);
        return results;
    }
}
