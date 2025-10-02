# JIRA Client

Internal client for JIRA Data Center REST API v2.

## Configuration

Set environment variables:
- `JIRA_BASE_URL`: JIRA instance URL (e.g., https://jira.example.com)
- `JIRA_PAT`: Personal Access Token for authentication

## Usage

### Direct Usage

```csharp
using SdlcAutomation.Clients.Jira;

// Create client
using var client = JiraApiClient.CreateFromEnvironment(baseUrl);

// Create user story
var story = await client.CreateUserStoryAsync(
    projectKey: "PROJ",
    summary: "Story title",
    description: "Story description");

// Create test item
var test = await client.CreateTestItemAsync(
    projectKey: "PROJ",
    summary: "Test title",
    description: "Test description");

// Create any issue type
var issue = await client.CreateIssueAsync(
    projectKey: "PROJ",
    issueTypeName: "Bug",
    summary: "Bug title",
    description: "Bug description");
```

### Dependency Injection (for API/Web apps)

```csharp
using SdlcAutomation.Clients.Jira;

// In Startup.cs or Program.cs
services.AddJiraClient(); // Uses JIRA_BASE_URL and JIRA_PAT env vars

// Or with explicit values
services.AddJiraClient("https://jira.example.com", "your-token");

// Or with custom env var names
services.AddJiraClient("CUSTOM_JIRA_URL", "CUSTOM_JIRA_TOKEN");

// Then inject in controllers/services
public class MyService
{
    private readonly JiraApiClient _jiraClient;

    public MyService(JiraApiClient jiraClient)
    {
        _jiraClient = jiraClient;
    }
}
```

## Notes

- Uses camelCase JSON naming policy
- No external dependencies beyond .NET standard libraries
- Compatible with JIRA Data Center 8.x and 9.x
- Includes DataAnnotations validation for command requests
