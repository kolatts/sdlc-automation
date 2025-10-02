using System.ComponentModel.DataAnnotations;

namespace SdlcAutomation.Clients.Jira.Models;

/// <summary>
/// Represents a JIRA user
/// </summary>
public class User
{
    [Display(Name = "Account ID")]
    public string? AccountId { get; set; }
    
    [Display(Name = "Username")]
    public string? Name { get; set; }
    
    [Display(Name = "Display Name")]
    public string? DisplayName { get; set; }
    
    [Display(Name = "Email Address")]
    public string? EmailAddress { get; set; }
}
