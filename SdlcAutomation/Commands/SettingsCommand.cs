using System.CommandLine;
using System.Reflection;
using SdlcAutomation.Models;
using SdlcAutomation.Services;
using Spectre.Console;

namespace SdlcAutomation.Commands;

/// <summary>
/// Command for managing CLI user settings
/// </summary>
public class SettingsCommand : BaseCommand
{
    private readonly CliSettingsService _settingsService;

    public SettingsCommand() : base("settings", "Manage CLI user settings")
    {
        _settingsService = new CliSettingsService();

        AddCommand(CreateShowCommand());
        AddCommand(CreateEditCommand());
        AddCommand(CreateResetCommand());
    }

    private Command CreateShowCommand()
    {
        var showCommand = new Command("show", "Show current settings");
        showCommand.SetHandler(ShowSettingsAsync);
        return showCommand;
    }

    private Command CreateEditCommand()
    {
        var editCommand = new Command("edit", "Edit settings interactively");
        editCommand.SetHandler(EditSettingsAsync);
        return editCommand;
    }

    private Command CreateResetCommand()
    {
        var resetCommand = new Command("reset", "Reset settings to defaults");
        resetCommand.SetHandler(ResetSettingsAsync);
        return resetCommand;
    }

    private async Task ShowSettingsAsync()
    {
        try
        {
            var settings = await Logger.ExecuteWithSpinnerAsync("Loading settings", 
                async () => await _settingsService.LoadSettingsAsync());

            var panel = new Panel(BuildSettingsDetails(settings))
            {
                Header = new PanelHeader("[bold]CLI User Settings[/]"),
                Border = BoxBorder.Rounded
            };

            AnsiConsole.Write(panel);
            Logger.WriteInfo($"\nConfig file: {_settingsService.GetConfigPath()}");
        }
        catch (Exception ex)
        {
            Logger.WriteError($"Failed to show settings: {ex.Message}");
        }
    }

    private string BuildSettingsDetails(CliUserSettings settings)
    {
        var details = new List<string>();

        details.Add($"[bold cyan]General Settings:[/]");
        details.Add($"  Default Organization: {settings.DefaultOrganization ?? "[dim]Not set[/]"}");
        details.Add($"  Verbose Logging: {(settings.VerboseLogging ? "[green]Yes[/]" : "[dim]No[/]")}");
        details.Add($"  Show Timings: {(settings.ShowTimings ? "[green]Yes[/]" : "[dim]No[/]")}");
        details.Add("");

        details.Add($"[bold yellow]Organizations:[/] {settings.Organizations.Count} configured");
        if (settings.Organizations.Any())
        {
            foreach (var org in settings.Organizations.OrderBy(o => o.Name).Take(5))
            {
                details.Add($"  • {org.Name}");
            }
            if (settings.Organizations.Count > 5)
            {
                details.Add($"  [dim]...and {settings.Organizations.Count - 5} more[/]");
            }
        }
        else
        {
            details.Add($"  [dim]No organizations configured[/]");
        }

        details.Add("");
        details.Add($"[dim]Created: {settings.CreatedAt.ToLocalTime():yyyy-MM-dd HH:mm:ss}[/]");
        details.Add($"[dim]Updated: {settings.UpdatedAt.ToLocalTime():yyyy-MM-dd HH:mm:ss}[/]");

        return string.Join("\n", details);
    }

    private async Task EditSettingsAsync()
    {
        try
        {
            var settings = await _settingsService.LoadSettingsAsync();
            var modified = false;

            while (true)
            {
                AnsiConsole.Clear();
                
                var panel = new Panel(BuildSettingsDetails(settings))
                {
                    Header = new PanelHeader("[bold]Edit CLI User Settings[/]"),
                    Border = BoxBorder.Rounded
                };
                AnsiConsole.Write(panel);

                var choices = new List<string>
                {
                    "Set Default Organization",
                    "Toggle Verbose Logging",
                    "Toggle Show Timings",
                    modified ? "[green]Save & Exit[/]" : "Exit without saving"
                };

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("\n[cyan]What would you like to do?[/]")
                        .PageSize(10)
                        .AddChoices(choices));

                if (choice == "Set Default Organization")
                {
                    if (!settings.Organizations.Any())
                    {
                        Logger.WriteWarning("No organizations configured. Add one with 'sdlc org add' first.");
                        AnsiConsole.Markup("\n[dim]Press any key to continue...[/]");
                        Console.ReadKey(true);
                        continue;
                    }

                    var orgChoices = settings.Organizations.Select(o => o.Name).OrderBy(n => n).ToList();
                    orgChoices.Insert(0, "[dim](Clear default)[/]");

                    var selectedOrg = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[cyan]Select default organization:[/]")
                            .PageSize(10)
                            .AddChoices(orgChoices));

                    if (selectedOrg == "[dim](Clear default)[/]")
                    {
                        settings.DefaultOrganization = null;
                    }
                    else
                    {
                        settings.DefaultOrganization = selectedOrg;
                    }
                    modified = true;
                }
                else if (choice == "Toggle Verbose Logging")
                {
                    settings.VerboseLogging = !settings.VerboseLogging;
                    modified = true;
                }
                else if (choice == "Toggle Show Timings")
                {
                    settings.ShowTimings = !settings.ShowTimings;
                    modified = true;
                }
                else if (choice.Contains("Save") || choice.Contains("Exit"))
                {
                    if (modified && choice.Contains("Save"))
                    {
                        await Logger.ExecuteWithSpinnerAsync("Saving settings",
                            async () => await _settingsService.SaveSettingsAsync(settings));
                        Logger.WriteSuccess("Settings saved successfully!");
                    }
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Logger.WriteError($"Failed to edit settings: {ex.Message}");
        }
    }

    private async Task ResetSettingsAsync()
    {
        try
        {
            var confirm = AnsiConsole.Confirm(
                "[yellow]Are you sure you want to reset all settings to defaults? This will not delete organizations.[/]",
                false);

            if (!confirm)
            {
                Logger.WriteInfo("Reset cancelled.");
                return;
            }

            var currentSettings = await _settingsService.LoadSettingsAsync();
            var newSettings = new CliUserSettings
            {
                Organizations = currentSettings.Organizations // Preserve organizations
            };

            await Logger.ExecuteWithSpinnerAsync("Resetting settings",
                async () => await _settingsService.SaveSettingsAsync(newSettings));

            Logger.WriteSuccess("Settings reset to defaults (organizations preserved).");
        }
        catch (Exception ex)
        {
            Logger.WriteError($"Failed to reset settings: {ex.Message}");
        }
    }
}
