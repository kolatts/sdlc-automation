using System.Text.Json;
using SdlcAutomation.Models;

namespace SdlcAutomation.Services;

/// <summary>
/// Service for managing organization configurations
/// </summary>
public class OrganizationConfigService
{
    private readonly string _configPath;
    private readonly JsonSerializerOptions _jsonOptions;

    public OrganizationConfigService()
    {
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var sdlcDir = Path.Combine(homeDir, ".sdlc");
        Directory.CreateDirectory(sdlcDir);
        _configPath = Path.Combine(sdlcDir, "organizations.json");

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <summary>
    /// Load all organizations from the configuration file
    /// </summary>
    public async Task<List<OrganizationConfig>> LoadOrganizationsAsync()
    {
        if (!File.Exists(_configPath))
        {
            return new List<OrganizationConfig>();
        }

        try
        {
            var json = await File.ReadAllTextAsync(_configPath);
            var organizations = JsonSerializer.Deserialize<List<OrganizationConfig>>(json, _jsonOptions);
            return organizations ?? new List<OrganizationConfig>();
        }
        catch (JsonException)
        {
            // If file is corrupted, return empty list
            return new List<OrganizationConfig>();
        }
    }

    /// <summary>
    /// Save all organizations to the configuration file
    /// </summary>
    public async Task SaveOrganizationsAsync(List<OrganizationConfig> organizations)
    {
        var json = JsonSerializer.Serialize(organizations, _jsonOptions);
        await File.WriteAllTextAsync(_configPath, json);
    }

    /// <summary>
    /// Add or update an organization
    /// </summary>
    public async Task<OrganizationConfig> SaveOrganizationAsync(OrganizationConfig organization)
    {
        var organizations = await LoadOrganizationsAsync();
        var existing = organizations.FirstOrDefault(o => 
            o.Name.Equals(organization.Name, StringComparison.OrdinalIgnoreCase));

        if (existing != null)
        {
            // Update existing
            organization.CreatedAt = existing.CreatedAt;
            organization.UpdatedAt = DateTime.UtcNow;
            organizations.Remove(existing);
        }
        else
        {
            // New organization
            organization.CreatedAt = DateTime.UtcNow;
            organization.UpdatedAt = DateTime.UtcNow;
        }

        organizations.Add(organization);
        await SaveOrganizationsAsync(organizations);
        return organization;
    }

    /// <summary>
    /// Get an organization by name
    /// </summary>
    public async Task<OrganizationConfig?> GetOrganizationAsync(string name)
    {
        var organizations = await LoadOrganizationsAsync();
        return organizations.FirstOrDefault(o => 
            o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Delete an organization
    /// </summary>
    public async Task<bool> DeleteOrganizationAsync(string name)
    {
        var organizations = await LoadOrganizationsAsync();
        var organization = organizations.FirstOrDefault(o => 
            o.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (organization == null)
        {
            return false;
        }

        organizations.Remove(organization);
        await SaveOrganizationsAsync(organizations);
        return true;
    }

    /// <summary>
    /// Get the path to the configuration file
    /// </summary>
    public string GetConfigPath() => _configPath;
}
