# SDLC Automation Tool

Command-line tool for SDLC automation tasks.

## Structure

- **Commands/**: Command implementations
  - **BaseCommand.cs**: Base class with helper methods for console output
  - **RootCommand.cs**: Root command configuration
  - **GitHubCommand.cs**: GitHub integration commands
  - **JiraCommand.cs**: JIRA integration commands
  - **AzureDevOpsCommand.cs**: Azure DevOps integration commands
- **Program.cs**: Application entry point

## Dependencies

- **SdlcAutomation.GitHub**: GitHub REST API client (optional)
- **SdlcAutomation.Jira**: JIRA Data Center REST API client (optional)
- **SdlcAutomation.AzureDevOps**: Azure DevOps REST API client (optional)

## Running

```bash
# Show help
dotnet run -- --help

# GitHub commands
dotnet run -- github --help
dotnet run -- github create --help

# JIRA commands
dotnet run -- jira --help
dotnet run -- jira create --help
```

## Adding Commands

Create a class extending `BaseCommand`:

```csharp
public class MyCommand : BaseCommand
{
    public MyCommand() : base("mycommand", "Description")
    {
        this.SetHandler(() => Execute());
    }

    private void Execute()
    {
        WriteSuccess("Command executed successfully!");
    }
}
```

Add to `RootCommand`:
```csharp
AddCommand(new MyCommand());
```
