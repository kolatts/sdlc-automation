using System.ComponentModel.DataAnnotations;

namespace SdlcAutomation.Jira.Models;

/// <summary>
/// Represents a JIRA priority
/// </summary>
public class Priority
{
    [Display(Name = "Priority ID")]
    public string? Id { get; set; }
    
    [Display(Name = "Priority")]
    public string? Name { get; set; }
}
