namespace SdlcAutomation.Clients.Jira.Models;

/// <summary>
/// Represents a JIRA user
/// </summary>
public class User
{
    public string? AccountId { get; set; }
    public string? Name { get; set; }
    public string? DisplayName { get; set; }
    public string? EmailAddress { get; set; }
}
