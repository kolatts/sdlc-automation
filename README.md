# SDLC Automation

Command-line tool for SDLC automation tasks.

## Project Structure

- **SdlcAutomation**: Main CLI application
- **SdlcAutomation.Jira**: JIRA Data Center REST API client library
- **SdlcAutomation.AzureDevOps**: Azure DevOps REST API client library

The client libraries are separate projects that can be optionally referenced, with each library managing its own package dependencies.

## Building

```bash
dotnet build SdlcAutomation/SdlcAutomation.csproj
```

Or on Windows:
```powershell
dotnet build SdlcAutomation\SdlcAutomation.csproj
```

## Usage

```bash
dotnet run --project SdlcAutomation -- --help
```

### JIRA Integration

Create work items in JIRA:

```bash
# Set environment variables (on Windows, use `set` or `$env:`)
export JIRA_BASE_URL="https://jira.example.com"
export JIRA_PAT="your-personal-access-token"

# Create a user story
dotnet run --project SdlcAutomation -- jira create --project PROJ --type Story --summary "My story" --description "Story details"

# Create a test item
dotnet run --project SdlcAutomation -- jira create --project PROJ --type Test --summary "Test case" --description "Test details"
```

### Migrating from Azure DevOps to JIRA

Convert an Azure DevOps work item to a JIRA issue:

```bash
# Set environment variables for both Azure DevOps and JIRA
export AZURE_DEVOPS_PAT="your-ado-pat-token"
export JIRA_BASE_URL="https://jira.example.com"
export JIRA_PAT="your-jira-personal-access-token"

# Convert an Azure DevOps work item to JIRA
dotnet run --project SdlcAutomation -- jira ado-to-jira \
  --ado-organization https://dev.azure.com/your-org \
  --ado-project YourProject \
  --work-item-id 12345 \
  --jira-project PROJ \
  --jira-issue-type Story
```

The command will:
- Fetch the work item from Azure DevOps
- Create a new JIRA issue with the work item's title and description
- Set both assignee and reporter to the current JIRA user (authenticated via PAT)
- Include acceptance criteria and metadata about the original work item

## License

Apache License 2.0
