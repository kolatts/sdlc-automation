# SDLC Automation

Command-line tool for SDLC automation tasks.

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

## License

Apache License 2.0
