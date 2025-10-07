using System.Text.Json;
using SdlcAutomation.Models;

namespace SdlcAutomation.Services;

/// <summary>
/// Service for managing CLI user settings
/// </summary>
public class CliSettingsService
{
    private readonly string _configPath;
    private readonly JsonSerializerOptions _jsonOptions;
    private CliUserSettings? _cachedSettings;

    public CliSettingsService()
    {
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var sdlcDir = Path.Combine(homeDir, ".sdlc");
        Directory.CreateDirectory(sdlcDir);
        _configPath = Path.Combine(sdlcDir, "settings.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <summary>
    /// Load settings from the configuration file
    /// </summary>
    public async Task<CliUserSettings> LoadSettingsAsync()
    {
        if (_cachedSettings != null)
        {
            return _cachedSettings;
        }

        if (!File.Exists(_configPath))
        {
            _cachedSettings = new CliUserSettings();
            return _cachedSettings;
        }

        try
        {
            var json = await File.ReadAllTextAsync(_configPath);
            _cachedSettings = JsonSerializer.Deserialize<CliUserSettings>(json, _jsonOptions) ?? new CliUserSettings();
            return _cachedSettings;
        }
        catch (JsonException)
        {
            // If file is corrupted, return new settings
            _cachedSettings = new CliUserSettings();
            return _cachedSettings;
        }
    }

    /// <summary>
    /// Save settings to the configuration file
    /// </summary>
    public async Task SaveSettingsAsync(CliUserSettings settings)
    {
        settings.UpdatedAt = DateTime.UtcNow;
        var json = JsonSerializer.Serialize(settings, _jsonOptions);
        await File.WriteAllTextAsync(_configPath, json);
        _cachedSettings = settings;
    }

    /// <summary>
    /// Get an organization by name
    /// </summary>
    public async Task<OrganizationConfig?> GetOrganizationAsync(string name)
    {
        var settings = await LoadSettingsAsync();
        return settings.Organizations.FirstOrDefault(o => 
            o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Add or update an organization
    /// </summary>
    public async Task<OrganizationConfig> SaveOrganizationAsync(OrganizationConfig organization)
    {
        var settings = await LoadSettingsAsync();
        var existing = settings.Organizations.FirstOrDefault(o => 
            o.Name.Equals(organization.Name, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
        {
            // Update existing
            organization.CreatedAt = existing.CreatedAt;
            organization.UpdatedAt = DateTime.UtcNow;
            settings.Organizations.Remove(existing);
        }
        else
        {
            // New organization
            organization.CreatedAt = DateTime.UtcNow;
            organization.UpdatedAt = DateTime.UtcNow;
        }

        settings.Organizations.Add(organization);
        await SaveSettingsAsync(settings);
        return organization;
    }

    /// <summary>
    /// Delete an organization
    /// </summary>
    public async Task<bool> DeleteOrganizationAsync(string name)
    {
        var settings = await LoadSettingsAsync();
        var organization = settings.Organizations.FirstOrDefault(o => 
            o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (organization == null)
        {
            return false;
        }

        settings.Organizations.Remove(organization);
        await SaveSettingsAsync(settings);
        return true;
    }

    /// <summary>
    /// Get the path to the configuration file
    /// </summary>
    public string GetConfigPath() => _configPath;

    /// <summary>
    /// Clear the cached settings
    /// </summary>
    public void ClearCache()
    {
        _cachedSettings = null;
    }
}
