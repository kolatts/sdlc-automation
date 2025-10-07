namespace SdlcAutomation.Models;

/// <summary>
/// General CLI user settings container
/// </summary>
public class CliUserSettings
{
    /// <summary>
    /// List of configured organizations
    /// </summary>
    public List<OrganizationConfig> Organizations { get; set; } = new();

    /// <summary>
    /// Default organization name to use when not specified
    /// </summary>
    public string? DefaultOrganization { get; set; }

    /// <summary>
    /// Enable verbose logging
    /// </summary>
    public bool VerboseLogging { get; set; }

    /// <summary>
    /// Show execution timing information
    /// </summary>
    public bool ShowTimings { get; set; } = true;

    /// <summary>
    /// Timestamp when settings were created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Timestamp when settings were last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
