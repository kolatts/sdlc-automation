# SDLC Automation

Command-line tool for SDLC automation tasks.

## Project Structure

- **SdlcAutomation**: Main CLI application
- **SdlcAutomation.Jira**: JIRA Data Center REST API client library
- **SdlcAutomation.AzureDevOps**: Azure DevOps REST API client library
- **SdlcAutomation.GitHub**: GitHub REST API client library

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

### GitHub Integration

Create issues in GitHub:

```bash
# Set environment variable (on Windows, use `set` or `$env:`)
export GITHUB_PAT="your-personal-access-token"

# Create an issue
dotnet run --project SdlcAutomation -- github create --owner username --repo repository --title "Issue title" --body "Issue description"

# Create an issue with labels and assignees
dotnet run --project SdlcAutomation -- github create --owner username --repo repository --title "Bug report" --body "Bug details" --labels "bug,priority-high" --assignees "developer1,developer2"
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

## License

Apache License 2.0
