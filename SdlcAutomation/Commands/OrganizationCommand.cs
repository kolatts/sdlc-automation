using System.CommandLine;
using SdlcAutomation.Models;
using SdlcAutomation.Services;
using Spectre.Console;

namespace SdlcAutomation.Commands;

/// <summary>
/// Command for managing organization configurations
/// </summary>
public class OrganizationCommand : BaseCommand
{
    private readonly CliSettingsService _settingsService;

    public OrganizationCommand() : base("org", "Manage organization configurations")
    {
        _settingsService = new CliSettingsService();

        // Add subcommands
        AddCommand(CreateAddCommand());
        AddCommand(CreateListCommand());
        AddCommand(CreateShowCommand());
        AddCommand(CreateRemoveCommand());
        AddCommand(CreateSetGitHubCommand());
        AddCommand(CreateSetJiraCommand());
        AddCommand(CreateSetAzureDevOpsCommand());
    }

    private Command CreateAddCommand()
    {
        var addCommand = new Command("add", "Add a new organization");

        var nameOption = new Option<string>(
            "--name",
            "Organization name") { IsRequired = true };

        var descriptionOption = new Option<string?>(
            "--description",
            "Organization description");

        addCommand.AddOption(nameOption);
        addCommand.AddOption(descriptionOption);

        addCommand.SetHandler(AddOrganizationAsync, nameOption, descriptionOption);

        return addCommand;
    }

    private Command CreateListCommand()
    {
        var listCommand = new Command("list", "List all organizations");
        listCommand.SetHandler(ListOrganizationsAsync);
        return listCommand;
    }

    private Command CreateShowCommand()
    {
        var showCommand = new Command("show", "Show organization details");

        var nameOption = new Option<string>(
            "--name",
            "Organization name") { IsRequired = true };

        showCommand.AddOption(nameOption);
        showCommand.SetHandler(ShowOrganizationAsync, nameOption);

        return showCommand;
    }

    private Command CreateRemoveCommand()
    {
        var removeCommand = new Command("remove", "Remove an organization");

        var nameOption = new Option<string>(
            "--name",
            "Organization name") { IsRequired = true };

        removeCommand.AddOption(nameOption);
        removeCommand.SetHandler(RemoveOrganizationAsync, nameOption);

        return removeCommand;
    }

    private Command CreateSetGitHubCommand()
    {
        var setGitHubCommand = new Command("set-github", "Configure GitHub settings for an organization");

        var nameOption = new Option<string>(
            "--name",
            "Organization name") { IsRequired = true };

        var orgOption = new Option<string?>(
            "--organization",
            "GitHub organization name");

        var baseUrlOption = new Option<string?>(
            "--base-url",
            "GitHub API base URL (for GitHub Enterprise)");

        var patEnvOption = new Option<string?>(
            "--pat-env",
            "Environment variable name for GitHub PAT");

        setGitHubCommand.AddOption(nameOption);
        setGitHubCommand.AddOption(orgOption);
        setGitHubCommand.AddOption(baseUrlOption);
        setGitHubCommand.AddOption(patEnvOption);

        setGitHubCommand.SetHandler(SetGitHubConfigAsync, nameOption, orgOption, baseUrlOption, patEnvOption);

        return setGitHubCommand;
    }

    private Command CreateSetJiraCommand()
    {
        var setJiraCommand = new Command("set-jira", "Configure Jira settings for an organization");

        var nameOption = new Option<string>(
            "--name",
            "Organization name") { IsRequired = true };

        var baseUrlOption = new Option<string?>(
            "--base-url",
            "Jira instance base URL");

        var patEnvOption = new Option<string?>(
            "--pat-env",
            "Environment variable name for Jira PAT");

        setJiraCommand.AddOption(nameOption);
        setJiraCommand.AddOption(baseUrlOption);
        setJiraCommand.AddOption(patEnvOption);

        setJiraCommand.SetHandler(SetJiraConfigAsync, nameOption, baseUrlOption, patEnvOption);

        return setJiraCommand;
    }

    private Command CreateSetAzureDevOpsCommand()
    {
        var setAdoCommand = new Command("set-ado", "Configure Azure DevOps settings for an organization");

        var nameOption = new Option<string>(
            "--name",
            "Organization name") { IsRequired = true };

        var orgUrlOption = new Option<string?>(
            "--organization-url",
            "Azure DevOps organization URL");

        var projectOption = new Option<string?>(
            "--default-project",
            "Default project name");

        var patEnvOption = new Option<string?>(
            "--pat-env",
            "Environment variable name for Azure DevOps PAT");

        setAdoCommand.AddOption(nameOption);
        setAdoCommand.AddOption(orgUrlOption);
        setAdoCommand.AddOption(projectOption);
        setAdoCommand.AddOption(patEnvOption);

        setAdoCommand.SetHandler(SetAzureDevOpsConfigAsync, nameOption, orgUrlOption, projectOption, patEnvOption);

        return setAdoCommand;
    }

    private async Task AddOrganizationAsync(string name, string? description)
    {
        try
        {
            await ExecuteWithTimingAsync("Add organization", async () =>
            {
                // Check if organization already exists
                var existing = await _settingsService.GetOrganizationAsync(name);
                if (existing != null)
                {
                    WriteWarning($"Organization '{name}' already exists. Use 'org show --name {name}' to view it.");
                    return;
                }

                var org = new OrganizationConfig
                {
                    Name = name,
                    Description = description
                };

                await _settingsService.SaveOrganizationAsync(org);
                WriteSuccess($"Added organization: {name}");
                WriteInfo($"Configuration saved to: {_settingsService.GetConfigPath()}");
                WriteInfo($"\nNext steps:");
                WriteInfo($"  • Configure GitHub: sdlc org set-github --name {name}");
                WriteInfo($"  • Configure Jira: sdlc org set-jira --name {name}");
                WriteInfo($"  • Configure Azure DevOps: sdlc org set-ado --name {name}");
            });
        }
        catch (Exception ex)
        {
            WriteError($"Failed to add organization: {ex.Message}");
        }
    }

    private async Task ListOrganizationsAsync()
    {
        try
        {
            await ExecuteWithTimingAsync("List organizations", async () =>
            {
                var settings = await _settingsService.LoadSettingsAsync();
                var organizations = settings.Organizations;

                if (!organizations.Any())
                {
                    WriteWarning("No organizations configured.");
                    WriteInfo("Use 'sdlc org add --name <name>' to add an organization.");
                    return;
                }

                var table = new Table();
                table.AddColumn("Name");
                table.AddColumn("Description");
                table.AddColumn("GitHub");
                table.AddColumn("Jira");
                table.AddColumn("Azure DevOps");
                table.AddColumn("Created");

                foreach (var org in organizations.OrderBy(o => o.Name))
                {
                    table.AddRow(
                        org.Name,
                        org.Description ?? "-",
                        org.GitHub != null ? "✓" : "-",
                        org.Jira != null ? "✓" : "-",
                        org.AzureDevOps != null ? "✓" : "-",
                        org.CreatedAt.ToLocalTime().ToString("yyyy-MM-dd")
                    );
                }

                AnsiConsole.Write(table);
                WriteInfo($"\nTotal: {organizations.Count} organization(s)");
                WriteInfo($"Config file: {_settingsService.GetConfigPath()}");
            });
        }
        catch (Exception ex)
        {
            WriteError($"Failed to list organizations: {ex.Message}");
        }
    }

    private async Task ShowOrganizationAsync(string name)
    {
        try
        {
            await ExecuteWithTimingAsync("Show organization", async () =>
            {
                var org = await _settingsService.GetOrganizationAsync(name);

                if (org == null)
                {
                    WriteError($"Organization '{name}' not found.");
                    WriteInfo("Use 'sdlc org list' to see all organizations.");
                    return;
                }

                var panel = new Panel(BuildOrganizationDetails(org))
                {
                    Header = new PanelHeader($"[bold]{org.Name}[/]"),
                    Border = BoxBorder.Rounded
                };

                AnsiConsole.Write(panel);
            });
        }
        catch (Exception ex)
        {
            WriteError($"Failed to show organization: {ex.Message}");
        }
    }

    private string BuildOrganizationDetails(OrganizationConfig org)
    {
        var details = new List<string>();

        if (!string.IsNullOrEmpty(org.Description))
        {
            details.Add($"[bold]Description:[/] {org.Description}");
            details.Add("");
        }

        // GitHub configuration
        if (org.GitHub != null)
        {
            details.Add("[bold cyan]GitHub Configuration:[/]");
            if (!string.IsNullOrEmpty(org.GitHub.Organization))
                details.Add($"  Organization: {org.GitHub.Organization}");
            if (!string.IsNullOrEmpty(org.GitHub.BaseUrl))
                details.Add($"  Base URL: {org.GitHub.BaseUrl}");
            details.Add($"  PAT Environment Variable: {org.GitHub.PatEnvironmentVariable}");
            var githubPat = Environment.GetEnvironmentVariable(org.GitHub.PatEnvironmentVariable);
            details.Add($"  PAT Status: {(string.IsNullOrEmpty(githubPat) ? "[red]Not Set[/]" : "[green]Set[/]")}");
            details.Add("");
        }

        // Jira configuration
        if (org.Jira != null)
        {
            details.Add("[bold yellow]Jira Configuration:[/]");
            if (!string.IsNullOrEmpty(org.Jira.BaseUrl))
                details.Add($"  Base URL: {org.Jira.BaseUrl}");
            details.Add($"  PAT Environment Variable: {org.Jira.PatEnvironmentVariable}");
            var jiraPat = Environment.GetEnvironmentVariable(org.Jira.PatEnvironmentVariable);
            details.Add($"  PAT Status: {(string.IsNullOrEmpty(jiraPat) ? "[red]Not Set[/]" : "[green]Set[/]")}");
            details.Add("");
        }

        // Azure DevOps configuration
        if (org.AzureDevOps != null)
        {
            details.Add("[bold blue]Azure DevOps Configuration:[/]");
            if (!string.IsNullOrEmpty(org.AzureDevOps.OrganizationUrl))
                details.Add($"  Organization URL: {org.AzureDevOps.OrganizationUrl}");
            if (!string.IsNullOrEmpty(org.AzureDevOps.DefaultProject))
                details.Add($"  Default Project: {org.AzureDevOps.DefaultProject}");
            details.Add($"  PAT Environment Variable: {org.AzureDevOps.PatEnvironmentVariable}");
            var adoPat = Environment.GetEnvironmentVariable(org.AzureDevOps.PatEnvironmentVariable);
            details.Add($"  PAT Status: {(string.IsNullOrEmpty(adoPat) ? "[red]Not Set[/]" : "[green]Set[/]")}");
            details.Add("");
        }

        details.Add($"[dim]Created: {org.CreatedAt.ToLocalTime():yyyy-MM-dd HH:mm:ss}[/]");
        details.Add($"[dim]Updated: {org.UpdatedAt.ToLocalTime():yyyy-MM-dd HH:mm:ss}[/]");

        return string.Join("\n", details);
    }

    private async Task RemoveOrganizationAsync(string name)
    {
        try
        {
            await ExecuteWithTimingAsync("Remove organization", async () =>
            {
                var removed = await _settingsService.DeleteOrganizationAsync(name);

                if (removed)
                {
                    WriteSuccess($"Removed organization: {name}");
                }
                else
                {
                    WriteError($"Organization '{name}' not found.");
                    WriteInfo("Use 'sdlc org list' to see all organizations.");
                }
            });
        }
        catch (Exception ex)
        {
            WriteError($"Failed to remove organization: {ex.Message}");
        }
    }

    private async Task SetGitHubConfigAsync(string name, string? organization, string? baseUrl, string? patEnv)
    {
        try
        {
            await ExecuteWithTimingAsync("Set GitHub configuration", async () =>
            {
                var org = await _settingsService.GetOrganizationAsync(name);

                if (org == null)
                {
                    WriteError($"Organization '{name}' not found.");
                    WriteInfo($"Use 'sdlc org add --name {name}' to create it first.");
                    return;
                }

                // Initialize or update GitHub config
                org.GitHub ??= new GitHubConfig();

                if (!string.IsNullOrEmpty(organization))
                    org.GitHub.Organization = organization;

                if (!string.IsNullOrEmpty(baseUrl))
                    org.GitHub.BaseUrl = baseUrl;

                if (!string.IsNullOrEmpty(patEnv))
                    org.GitHub.PatEnvironmentVariable = patEnv;
                else if (string.IsNullOrEmpty(org.GitHub.PatEnvironmentVariable))
                    org.GitHub.PatEnvironmentVariable = $"{name.ToUpperInvariant()}_GITHUB_PAT";

                await _settingsService.SaveOrganizationAsync(org);

                WriteSuccess($"GitHub configuration updated for organization: {name}");
                WriteInfo($"\nEnvironment variable for PAT: {org.GitHub.PatEnvironmentVariable}");
                
                var githubPat = Environment.GetEnvironmentVariable(org.GitHub.PatEnvironmentVariable);
                if (string.IsNullOrEmpty(githubPat))
                {
                    WriteWarning($"PAT not set. Set it with:");
                    WriteInfo($"  export {org.GitHub.PatEnvironmentVariable}=\"your-github-pat\"");
                }
                else
                {
                    WriteSuccess("PAT is configured.");
                }
            });
        }
        catch (Exception ex)
        {
            WriteError($"Failed to set GitHub configuration: {ex.Message}");
        }
    }

    private async Task SetJiraConfigAsync(string name, string? baseUrl, string? patEnv)
    {
        try
        {
            await ExecuteWithTimingAsync("Set Jira configuration", async () =>
            {
                var org = await _settingsService.GetOrganizationAsync(name);

                if (org == null)
                {
                    WriteError($"Organization '{name}' not found.");
                    WriteInfo($"Use 'sdlc org add --name {name}' to create it first.");
                    return;
                }

                // Initialize or update Jira config
                org.Jira ??= new JiraConfig();

                if (!string.IsNullOrEmpty(baseUrl))
                    org.Jira.BaseUrl = baseUrl;

                if (!string.IsNullOrEmpty(patEnv))
                    org.Jira.PatEnvironmentVariable = patEnv;
                else if (string.IsNullOrEmpty(org.Jira.PatEnvironmentVariable))
                    org.Jira.PatEnvironmentVariable = $"{name.ToUpperInvariant()}_JIRA_PAT";

                await _settingsService.SaveOrganizationAsync(org);

                WriteSuccess($"Jira configuration updated for organization: {name}");
                WriteInfo($"\nEnvironment variable for PAT: {org.Jira.PatEnvironmentVariable}");
                
                var jiraPat = Environment.GetEnvironmentVariable(org.Jira.PatEnvironmentVariable);
                if (string.IsNullOrEmpty(jiraPat))
                {
                    WriteWarning($"PAT not set. Set it with:");
                    WriteInfo($"  export {org.Jira.PatEnvironmentVariable}=\"your-jira-pat\"");
                }
                else
                {
                    WriteSuccess("PAT is configured.");
                }
            });
        }
        catch (Exception ex)
        {
            WriteError($"Failed to set Jira configuration: {ex.Message}");
        }
    }

    private async Task SetAzureDevOpsConfigAsync(string name, string? organizationUrl, string? defaultProject, string? patEnv)
    {
        try
        {
            await ExecuteWithTimingAsync("Set Azure DevOps configuration", async () =>
            {
                var org = await _settingsService.GetOrganizationAsync(name);

                if (org == null)
                {
                    WriteError($"Organization '{name}' not found.");
                    WriteInfo($"Use 'sdlc org add --name {name}' to create it first.");
                    return;
                }

                // Initialize or update Azure DevOps config
                org.AzureDevOps ??= new AzureDevOpsConfig();

                if (!string.IsNullOrEmpty(organizationUrl))
                    org.AzureDevOps.OrganizationUrl = organizationUrl;

                if (!string.IsNullOrEmpty(defaultProject))
                    org.AzureDevOps.DefaultProject = defaultProject;

                if (!string.IsNullOrEmpty(patEnv))
                    org.AzureDevOps.PatEnvironmentVariable = patEnv;
                else if (string.IsNullOrEmpty(org.AzureDevOps.PatEnvironmentVariable))
                    org.AzureDevOps.PatEnvironmentVariable = $"{name.ToUpperInvariant()}_AZURE_DEVOPS_PAT";

                await _settingsService.SaveOrganizationAsync(org);

                WriteSuccess($"Azure DevOps configuration updated for organization: {name}");
                WriteInfo($"\nEnvironment variable for PAT: {org.AzureDevOps.PatEnvironmentVariable}");
                
                var adoPat = Environment.GetEnvironmentVariable(org.AzureDevOps.PatEnvironmentVariable);
                if (string.IsNullOrEmpty(adoPat))
                {
                    WriteWarning($"PAT not set. Set it with:");
                    WriteInfo($"  export {org.AzureDevOps.PatEnvironmentVariable}=\"your-ado-pat\"");
                }
                else
                {
                    WriteSuccess("PAT is configured.");
                }
            });
        }
        catch (Exception ex)
        {
            WriteError($"Failed to set Azure DevOps configuration: {ex.Message}");
        }
    }
}
