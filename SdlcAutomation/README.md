# SDLC Automation Tool

Command-line tool for SDLC automation tasks.

## Structure

- **Commands/**: Command implementations
  - **BaseCommand.cs**: Base class with helper methods for console output and timing tracking
  - **RootCommand.cs**: Root command configuration
  - **OrganizationCommand.cs**: Organization configuration management
  - **JiraCommand.cs**: JIRA integration commands
  - **AzureDevOpsCommand.cs**: Azure DevOps integration commands
- **Models/**: Data models
  - **OrganizationConfig.cs**: Organization and service configuration models
- **Services/**: Service implementations
  - **OrganizationConfigService.cs**: Organization persistence service
- **Program.cs**: Application entry point

## Dependencies

- **SdlcAutomation.Jira**: JIRA Data Center REST API client (optional)
- **SdlcAutomation.AzureDevOps**: Azure DevOps REST API client (optional)

## Running

```bash
# Show help
dotnet run -- --help

# Organization commands
dotnet run -- org --help
dotnet run -- org add --name "MyOrg" --description "My organization"
dotnet run -- org list
dotnet run -- org show --name "MyOrg"

# JIRA commands
dotnet run -- jira --help
dotnet run -- jira create --help
```

## Organization Management

The tool supports managing multiple organization configurations with their associated GitHub, Jira, and Azure DevOps settings. Configurations are stored in `~/.sdlc/organizations.json`.

### Adding an Organization

```bash
dotnet run -- org add --name "MyCompany" --description "Main company organization"
```

### Configuring Services

```bash
# GitHub
dotnet run -- org set-github --name "MyCompany" \
  --organization "mycompany" \
  --base-url "https://api.github.com"

# Jira
dotnet run -- org set-jira --name "MyCompany" \
  --base-url "https://jira.mycompany.com"

# Azure DevOps
dotnet run -- org set-ado --name "MyCompany" \
  --organization-url "https://dev.azure.com/mycompany" \
  --default-project "MainProject"
```

### Setting PAT Environment Variables

Each organization uses keyed environment variables for authentication:

```bash
export MYCOMPANY_GITHUB_PAT="your-github-pat"
export MYCOMPANY_JIRA_PAT="your-jira-pat"
export MYCOMPANY_AZURE_DEVOPS_PAT="your-ado-pat"
```

The environment variable names follow the pattern: `{ORG_NAME}_{SERVICE}_PAT`

### Viewing Organizations

```bash
# List all organizations
dotnet run -- org list

# Show detailed configuration
dotnet run -- org show --name "MyCompany"
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
