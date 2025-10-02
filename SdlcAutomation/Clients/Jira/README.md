# JIRA Client

Internal client for JIRA Data Center REST API v2.

## Configuration

Set environment variables:
- `JIRA_BASE_URL`: JIRA instance URL (e.g., https://jira.example.com)
- `JIRA_PAT`: Personal Access Token for authentication

## Usage

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

## Notes

- Uses camelCase JSON naming policy
- No external dependencies beyond .NET standard libraries
- Compatible with JIRA Data Center 8.x and 9.x
