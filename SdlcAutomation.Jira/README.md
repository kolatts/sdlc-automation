# JIRA Client

Internal client for JIRA Data Center REST API v2.

## Configuration

Set environment variables:
- `JIRA_BASE_URL`: JIRA instance URL (e.g., https://jira.example.com)
- `JIRA_PAT`: Personal Access Token for authentication

## Usage

### Using the Unified Issue Model

The `Issue` model can be used for both creating and retrieving issues:

```csharp
using SdlcAutomation.Jira;
using SdlcAutomation.Jira.Models;

// Create client
using var client = JiraApiClient.CreateFromEnvironment(baseUrl);

// Create an issue using the unified model
var issue = new Issue
{
    Fields = new IssueFields
    {
        Project = new Project { Key = "PROJ" },
        IssueType = new IssueType { Name = "Story" },
        Summary = "Story title",
        Description = "Story description"
    }
};

// The model validates automatically
var createdIssue = await client.CreateIssueAsync(issue);

// Retrieve an issue - same model
var retrievedIssue = await client.GetIssueAsync("PROJ-123");
Console.WriteLine($"Summary: {retrievedIssue.Fields?.Summary}");
```

### Convenience Methods

```csharp
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
var bug = await client.CreateIssueAsync(
    projectKey: "PROJ",
    issueTypeName: "Bug",
    summary: "Bug title",
    description: "Bug description");
```

### Dependency Injection (for API/Web apps)

```csharp
using SdlcAutomation.Jira;

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

## Model Features

- **Unified Model**: Same `Issue` class for create and read operations
- **DataAnnotations Validation**: Built-in validation with clear error messages
- **Bidirectional**: Models work seamlessly for both API requests and responses
- **Extensible**: Support for custom fields via `CustomFields` dictionary

## Notes

- Uses camelCase JSON naming policy
- No external dependencies beyond .NET standard libraries
- Compatible with JIRA Data Center 8.x and 9.x
- Full DataAnnotations validation on all models
