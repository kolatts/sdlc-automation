# SDLC Automation Tool

Command-line tool for SDLC automation tasks.

## Structure

- **Commands/**: Command implementations
  - **BaseCommand.cs**: Base class with helper methods for console output
  - **RootCommand.cs**: Root command configuration
  - **JiraCommand.cs**: JIRA integration commands
  - **AzureDevOpsCommand.cs**: Azure DevOps integration commands
- **Program.cs**: Application entry point

## Dependencies

- **SdlcAutomation.Jira**: JIRA Data Center REST API client (optional)
- **SdlcAutomation.AzureDevOps**: Azure DevOps REST API client (optional)

## Running

```bash
# Show help
dotnet run -- --help

# JIRA commands
dotnet run -- jira --help
dotnet run -- jira create --help
dotnet run -- jira import-cucumber --help

# Azure DevOps commands
dotnet run -- ado --help
dotnet run -- ado query --help
dotnet run -- ado import-cucumber --help
```

## Cucumber Test Results Import

Import Cucumber test results to JIRA X-ray or Azure Test Plans:

### JIRA X-ray Import

```bash
# Import Cucumber JSON/NDJSON file to JIRA X-ray
dotnet run -- jira import-cucumber \
  --file path/to/cucumber-results.json \
  --work-item PROJ-123
```

The file should contain Cucumber test results in JSON or NDJSON format. The command sends the file content directly to JIRA X-ray's import endpoint without parsing or modification.

Requires `JIRA_BASE_URL` and `JIRA_PAT` environment variables.

### Azure Test Plans Import

```bash
# Import Cucumber test results to Azure Test Plans
dotnet run -- ado import-cucumber \
  --file path/to/cucumber-results.json \
  --work-item 12345 \
  --organization https://dev.azure.com/your-org \
  --project YourProject
```

Requires `AZURE_DEVOPS_PAT` environment variable.

**Note**: Azure Test Plans integration is a placeholder and requires additional implementation.

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
